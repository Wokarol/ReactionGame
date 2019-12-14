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
    [SerializeField, Range(1, 20)] private int maxVisibleCards = 5;

    [BoxGroup(animGroup), SerializeField] private float animationScale = 1;
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

    enum DebugAutoAnswerTypes { Correct, Wrong, FiftyFifty, Random }

    [Header("Debug Options")]
    [SerializeField] private bool autoAnswer = false;
    [SerializeField] private DebugAutoAnswerTypes autoAnswerType = DebugAutoAnswerTypes.FiftyFifty;

    // Animation State
    private bool animationRunning = false;
    private bool candidatesOnTable = false;
    private bool takesAnswer = true;
    private bool queuesAnswer = false;

    private int queuedAnswer = -1;

    private CardController lastModelCard;
    private Queue<CardController> modelCardQueue;
    private List<CardController> modelCardsList;
    private CardController[] candidatesCache = null;

    private Wokarol.GameplayCores.ReactionChooserCore<CardData> core;

    private void Awake()
    {
        modelCardQueue = new Queue<CardController>(maxVisibleCards);
        modelCardsList = new List<CardController>(maxVisibleCards);

        candidatesCache = new CardController[candidates.Length];
        for (int i = 0; i < candidatesCache.Length; i++) {
            int index = i;
            CardSpot spot = candidates[i];
            CardController card = Instantiate(cardPrefab, spot.transform.position + spot.StartingOffset, Quaternion.Euler(0, 0, Random.Range(-180, 180)));
            if (Application.isEditor)
                card.gameObject.name = $"{cardPrefab.name} Candidate [{i}]";

            candidatesCache[i] = card;

            card.OnClicked += () => Answer(index);
            card.SoringOrder = spot.SortingOrder;
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
#if UNITY_EDITOR
        if (autoAnswer && takesAnswer) {
            switch (autoAnswerType) {
                case DebugAutoAnswerTypes.Correct:
                    AnswerCorrect();
                    break;
                case DebugAutoAnswerTypes.Wrong:
                    AnswerWrong();
                    break;
                case DebugAutoAnswerTypes.FiftyFifty:
                    if (Random.value > 0.5f)
                        AnswerCorrect();
                    else
                        AnswerWrong();
                    break;
                case DebugAutoAnswerTypes.Random:
                    Answer(Random.Range(0, candidatesCache.Length));
                    break;
            }
        } 
#endif

        if (!animationRunning && queuedAnswer != -1) {
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

        Transform target = lastModelCard.transform;
        Transform card = candidatesCache[i].transform;

        float flyTime = correctTimeFlyTime * animationScale;

        card.DOMove(target.position, flyTime);
        card.DORotate(target.eulerAngles, flyTime, RotateMode.Fast);
        card.DOScale(target.localScale.x, flyTime)
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

        card.DOShakeRotation(failShakeDuration * animationScale, Vector3.forward * failShakeStrenght, failShakeVibrato, failShakeRandomness, false)
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

                float delay = cleanupTimeInterval * i * animationScale;
                var candSeq = DOTween.Sequence();
                candSeq.AppendInterval(delay);
                candSeq.Append(t.DOMove(spot.transform.position + spot.StartingOffset, cleanupTimePerCard * animationScale));
                candSeq.Join(t.DORotate(Vector3.forward * Random.Range(-180, 180), cleanupTimePerCard * animationScale, RotateMode.Fast));
                cleanupSequence.Join(candSeq);
            }
            seq.Append(cleanupSequence);
        }

        seq.AppendCallback(() => queuesAnswer = true);

        // Model Animation
        if (lastModelCard) {
            var c = lastModelCard;
            seq.AppendCallback(() => c.FadeIntoBack());
        }

        Transform modelCard = GetModelCard(modelData);

        var (modelPos, modelRot, _) = modelCardSpot.GetRandom();

        seq.Append(modelCard.DOMove(modelPos, patternTime * animationScale));
        seq.Join(modelCard.DORotate(Vector3.forward * modelRot, patternTime * animationScale));

        // Candidates Animation
        for (int i = 0; i < candidates.Length; i++) {
            var spot = candidates[i];
            CardController card = candidatesCache[i];
            var t = card.transform;
            int index = i;

            var (pos, rot, _) = spot.GetRandom();
            float delay = (patternTime * 0.5f + cardTimeInterval * i) * animationScale;

            var candSeq = DOTween.Sequence();

            candSeq.AppendInterval(delay);

            candSeq.AppendCallback(() => card.SetCard(candData[index]));

            candSeq.Append(t.DOMove(pos, cardTime * animationScale));
            candSeq.Join(t.DORotate(Vector3.forward * rot, cardTime * animationScale, RotateMode.Fast));

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
        CardController card = null;
        if (modelCardsList.Count < maxVisibleCards) {
            card = Instantiate(
                cardPrefab,
                modelCardSpot.transform.position + (Vector3.down * modelSpawnDistance),
                Quaternion.Euler(0, 0, Random.Range(-180f, 180f)));
            if(Application.isEditor)
                card.gameObject.name = $"{cardPrefab.name} Model [{modelCardsList.Count}]";
            modelCardsList.Add(card);
        } else {
            card = modelCardQueue.Dequeue();
            card.ResetFade();
            card.transform.SetPositionAndRotation(
                modelCardSpot.transform.position + (Vector3.down * modelSpawnDistance), 
                Quaternion.Euler(0, 0, Random.Range(-180f, 180f)));
            
            foreach (var c in modelCardsList) {
                c.SoringOrder -= 1;
            }

            card.SoringOrder = 0;
        }
        modelCardQueue.Enqueue(card);
        lastModelCard = card;
        card.SetCard(data);

        if(modelCardsList.Count >= maxVisibleCards) {
            modelCardQueue.Peek().FadeOut(animationScale);
        }

        Transform cardT = card.transform;
        cardT.localScale = Vector3.one * modelCardSpot.Scale;
        return cardT;
    }


    // Helper Methods
    [Button("Answer Correct", ButtonAttribute.EnableMode.Playmode)]
    private void AnswerCorrect()
    {
        for (int i = 0; i < core.Candidates.Count; i++) {
            if(core.Candidates[i] == core.Model) {
                Answer(i);
                return;
            }
        }
    }

    [Button("Answer Wrong", ButtonAttribute.EnableMode.Playmode)]
    private void AnswerWrong()
    {
        if(core.Candidates[0] != core.Model) {
            Answer(0);
        } else {
            Answer(1);
        }
    }
}
