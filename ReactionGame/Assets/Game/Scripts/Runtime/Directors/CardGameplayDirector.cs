using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CardGameplayDirector : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float modelSpawnDistance = 15;
    [Header("Resources")]
    [SerializeField] private CardController cardPrefab = null;
    [Header("Binds")]
    [SerializeField] private CardSpot modelCardSpot = null;
    [SerializeField] private CardSpot[] candidates = new CardSpot[0];

    // Animation State
    private bool animationRunning = false;
    private bool candidatesOnTable = false;
    private CardController lastCardModel = null;
    private CardController[] candidatesCache = null;

    private void Awake()
    {
        candidatesCache = new CardController[candidates.Length];
        for (int i = 0; i < candidatesCache.Length; i++) {
            CardSpot spot = candidates[i];
            candidatesCache[i] = Instantiate(cardPrefab, spot.transform.position + spot.StartingOffset, Quaternion.Euler(0, 0, Random.Range(-180, 180)));
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) {
            SpawnCards();
        }
    }

    // Add data
    void SpawnCards()
    {
        const float modelTime = 1;
        const float candidatesTime = 0.5f;
        const float candidatesInterval = 0.15f;
        const float candidatesTimeCleanup = 0.5f;
        const float candidatesIntervalCleanup = 0.1f;

        if (animationRunning)
            return;

        animationRunning = true;

        var seq = DOTween.Sequence();

        if (candidatesOnTable) {
            var cleanupSequence = DOTween.Sequence();
            for (int i = 0; i < candidates.Length; i++) {
                var spot = candidates[i];
                var t = candidatesCache[i].transform;

                float delay = candidatesIntervalCleanup * i;
                var candSeq = DOTween.Sequence();
                candSeq.AppendInterval(delay);
                candSeq.Append(t.DOMove(spot.transform.position + spot.StartingOffset, candidatesTimeCleanup));
                candSeq.Join(t.DORotate(Vector3.forward * Random.Range(-180, 180), candidatesTimeCleanup, RotateMode.Fast));
                cleanupSequence.Join(candSeq);
            }
            seq.Append(cleanupSequence);
        }

        // Model Animation
        if (lastCardModel) {
            var c = lastCardModel;
            seq.AppendCallback(() => c.SetActive(false));
        }

        Transform modelCard = GetModelCard();

        var (modelPos, modelRot, _) = modelCardSpot.GetRandom();

        seq.Append(modelCard.DOMove(modelPos, modelTime));
        seq.Join(modelCard.DORotate(Vector3.forward * modelRot, modelTime));

        // Candidates Animation
        for (int i = 0; i < candidates.Length; i++) {
            var spot = candidates[i];
            var t = candidatesCache[i].transform;

            var (pos, rot, _) = spot.GetRandom();
            float delay = modelTime * 0.5f + candidatesInterval * i;
            var candSeq = DOTween.Sequence();
            candSeq.AppendInterval(delay);
            candSeq.Append(t.DOMove(pos, candidatesTime));
            candSeq.Join(t.DORotate(Vector3.forward * rot, candidatesTime));
            seq.Join(candSeq);
        }
        candidatesOnTable = true;

        seq.AppendCallback(() => animationRunning = false);
    }

    private Transform GetModelCard()
    {
        var card = Instantiate(
            cardPrefab,
            modelCardSpot.transform.position + (Vector3.down * modelSpawnDistance),
            Quaternion.Euler(0, 0, Random.Range(-180f, 180f)));
        lastCardModel = card;

        Transform cardT = card.transform;
        cardT.localScale = Vector3.one * modelCardSpot.Scale;
        return cardT;
    }
}
