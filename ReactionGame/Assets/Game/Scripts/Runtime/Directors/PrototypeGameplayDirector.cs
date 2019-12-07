using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Wokarol.GameplayCores;

public class PrototypeGameplayDirector : MonoBehaviour
{
    public class ReferencedValue<T>
    {
        public T Value;
    }

    [SerializeField] private TMPro.TMP_Text modelText = null;
    [SerializeField] private SimpleButtonController[] buttons = null;
    [Space]
    [SerializeField] private ParticleSystem goodAnswer = null;
    [SerializeField] private ParticleSystem badAnswer = null;
    [Space]
    [SerializeField] private float roundStartTimer = 0.2f;

    private ReactionChooserCore<ReferencedValue<int>> core;

    private float countdown;
    private List<ReferencedValue<int>> candidates;

    public void Awake()
    {
        var allOptions = new List<ReferencedValue<int>>();
        for (int i = 0; i < 99; i++) {
            allOptions.Add(new ReferencedValue<int>() { Value = i });
        }
        core = new ReactionChooserCore<ReferencedValue<int>>(allOptions);
        core.OnNewTable += NewTableCreated;

        countdown = 0.1f;

        for (int i = 0; i < buttons.Length; i++) {
            int index = i;
            buttons[index].OnClicked += () => Answer(index);
        }
    }

    private void Start()
    {
        core.NewTable(buttons.Length);
    }

    private void Answer(int i)
    {
        countdown = roundStartTimer;
        bool result = core.Answer(candidates[i]);

        if (result) {
            goodAnswer.Play();
        } else {
            badAnswer.Play();
        }

        core.NewTable(buttons.Length);
    }

    //private void Update()
    //{
    //    if (countdown >= 0) {
    //        countdown -= Time.deltaTime;
    //        if (countdown < 0) {
    //            core.NewTable(buttons.Length);
    //        }
    //    }
    //}

    private void NewTableCreated(ReferencedValue<int> model, List<ReferencedValue<int>> candidates)
    {
        this.candidates = candidates;

        modelText.text = model.Value.ToString("00");
        for (int i = 0; i < candidates.Count; i++) {
            buttons[i].Text = candidates[i].Value.ToString("00");
        }
    }
}
