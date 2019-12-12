using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;
using Random = UnityEngine.Random;
using NaughtyAttributes;

public class CardGameplayDirector : MonoBehaviour
{
    private const string animGroup = "Animation";

    [Header("Settings")]
    [SerializeField] private float modelSpawnDistance = 15;
    [Header("Candidate's Cleanup Sequence")]
    [BoxGroup(animGroup), SerializeField] private float cleanupTimePerCard = 0.3f;
    [BoxGroup(animGroup), SerializeField] private float cleanupTimeInterval = 0.1f;
    [Header("New Table Show Sequence")]
    [BoxGroup(animGroup), SerializeField] private float patternTime = 0.5f;
    [BoxGroup(animGroup), SerializeField] private float cardTime = 0.3f;
    [BoxGroup(animGroup), SerializeField] private float cardTimeInterval = 0.15f;
    [Header("Correct Card Sequence")]
    [BoxGroup(animGroup), SerializeField] private float correctTimeFlyTime = 0.3f;
    [Header("Failed Card Sequence")]
    [BoxGroup(animGroup), SerializeField] private float failShakeDuration = 0.25f;
    [BoxGroup(animGroup), SerializeField] private float failShakeStrenght = 90;
    [BoxGroup(animGroup), SerializeField] private int failShakeVibrato = 10;
    [BoxGroup(animGroup), SerializeField] private float failShakeRandomness = 90;


    [Header("Resources")]
    [SerializeField] private CardController cardPrefab = null;
    [SerializeField] private List<CardData> CardData = new List<CardData>();
    [Header("Binds")]
    [SerializeField] private CardSpot modelCardSpot = null;
    [SerializeField] private CardSpot[] candidates = new CardSpot[0];

    // Animation State
    private bool animationRunning = false;
    private bool candidatesOnTable = false;
    private bool takesAnswer = true;
    private bool queuesAnswer = false;

    private int queuedAnswer = -1;

    private CardController lastCardModel = null;
    private CardController[] candidatesCache = null;

    private Wokarol.GameplayCores.ReactionChooserCore<CardData> core;

    private void Awake()
    {
        candidatesCache = new CardController[candidates.Length];
        for (int i = 0; i < candidatesCache.Length; i++) {
            int index = i;
            CardSpot spot = candidates[i];
            candidatesCache[i] = Instantiate(cardPrefab, spot.transform.position + spot.StartingOffset, Quaternion.Euler(0, 0, Random.Range(-180, 180)));
            candidatesCache[i].OnClicked += () => Answer(index);
        }

        core = new Wokarol.GameplayCores.ReactionChooserCore<CardData>(CardData);
        core.OnNewTable += SpawnCards;
    }

    private void OnDestroy()
    {
        core.OnNewTable -= SpawnCards;
    }

    private void Start()
    {
        NewTable();
    }

    void Update()
    {
        if(!animationRunning && queuedAnswer != -1) {
            Answer(queuedAnswer);
            queuedAnswer = -1;
        }
    }

    void Answer(int i)
    {
        if (takesAnswer) {
            takesAnswer = queuesAnswer = false;

            bool result = core.Answer(i);
            if (result) {
                PlayCorrectAnimation(i, NewTable);
            } else {
                PlayFailedAnimation(i, NewTable);
            }
        } else if(queuesAnswer) {
            Debug.Log("Queued!!!");
            queuesAnswer = false;
            queuedAnswer = i;
        }
    }

    void NewTable()
    {
        core.NewTable(candidates.Length);
    }

    void PlayCorrectAnimation(int i, Action callback)
    {
        CardSpot spot = candidates[i];

        if (animationRunning) {
            Debug.LogError("Animation started while other one wasn't winished");
        }
        animationRunning = true;

        Transform target = lastCardModel.transform;
        Transform card = candidatesCache[i].transform;

        card.DOMove(target.position, correctTimeFlyTime);
        card.DORotate(target.eulerAngles, correctTimeFlyTime, RotateMode.Fast);
        card.DOScale(target.localScale.x, correctTimeFlyTime)
            .OnComplete(Complete);

        void Complete()
        {
            card.position = spot.transform.position + spot.StartingOffset;
            card.localScale = Vector3.one * spot.Scale;
            animationRunning = false;
            callback();
        }
    }

    void PlayFailedAnimation(int i, Action callback)
    {
        CardSpot spot = candidates[i];

        if (animationRunning) {
            Debug.LogError("Animation started while other one wasn't winished");
        }
        animationRunning = true;

        Transform card = candidatesCache[i].transform;

        card.DOShakeRotation(failShakeDuration, Vector3.forward * failShakeStrenght, failShakeVibrato, failShakeRandomness, false)
            .OnComplete(Complete);

        void Complete()
        {
            animationRunning = false;
            callback();
        }
    }

    void SpawnCards(CardData modelData, List<CardData> candData)
    {
        if (animationRunning) {
            Debug.LogError("Animation started while other one wasn't winished");
        }
        animationRunning = true;

        var seq = DOTween.Sequence();

        // Cleans up candidates if needed
        if (candidatesOnTable) {
            var cleanupSequence = DOTween.Sequence();
            for (int i = 0; i < candidates.Length; i++) {
                var spot = candidates[i];
                var t = candidatesCache[i].transform;

                float delay = cleanupTimeInterval * i;
                var candSeq = DOTween.Sequence();
                candSeq.AppendInterval(delay);
                candSeq.Append(t.DOMove(spot.transform.position + spot.StartingOffset, cleanupTimePerCard));
                candSeq.Join(t.DORotate(Vector3.forward * Random.Range(-180, 180), cleanupTimePerCard, RotateMode.Fast));
                cleanupSequence.Join(candSeq);
            }
            seq.Append(cleanupSequence);
        }

        seq.AppendCallback(() => queuesAnswer = true);

        // Model Animation
        if (lastCardModel) {
            var c = lastCardModel;
            seq.AppendCallback(() => c.SetActive(false));
        }

        Transform modelCard = GetModelCard(modelData);

        var (modelPos, modelRot, _) = modelCardSpot.GetRandom();

        seq.Append(modelCard.DOMove(modelPos, patternTime));
        seq.Join(modelCard.DORotate(Vector3.forward * modelRot, patternTime));

        // Candidates Animation
        for (int i = 0; i < candidates.Length; i++) {
            var spot = candidates[i];
            CardController card = candidatesCache[i];
            var t = card.transform;
            int index = i;

            var (pos, rot, _) = spot.GetRandom();
            float delay = patternTime * 0.5f + cardTimeInterval * i;

            var candSeq = DOTween.Sequence();

            candSeq.AppendInterval(delay);

            candSeq.AppendCallback(() => card.SetCard(candData[index]));

            candSeq.Append(t.DOMove(pos, cardTime));
            candSeq.Join(t.DORotate(Vector3.forward * rot, cardTime, RotateMode.Fast));

            seq.Join(candSeq);
        }
        candidatesOnTable = true;

        seq.AppendCallback(() => {
            animationRunning = false;
            takesAnswer = true;
        });
    }

    private Transform GetModelCard(CardData data)
    {
        var card = Instantiate(
            cardPrefab,
            modelCardSpot.transform.position + (Vector3.down * modelSpawnDistance),
            Quaternion.Euler(0, 0, Random.Range(-180f, 180f)));
        lastCardModel = card;
        card.SetCard(data);

        Transform cardT = card.transform;
        cardT.localScale = Vector3.one * modelCardSpot.Scale;
        return cardT;
    }
}
