using System;
using System.Collections;
using System.Collections.Generic;
using Wokarol.MessageSystem;
using Wokarol.StateMachineSystem;

namespace Wokarol.GameplayCores
{
    public class ReactionChooserCore<CardT> where CardT : class
    {
        public delegate void TableCreated(CardT model, List<CardT> candidates);

        public event TableCreated OnNewTable;

        private List<CardT> all;

        // TableData
        public List<CardT> Candidates { get; private set; } = new List<CardT>();
        public CardT Model { get; private set; }

        public ReactionChooserCore(List<CardT> allCards)
        {
            all = allCards;
        }

        public void NewTable(int count)
        {
            Candidates.Clear();
            all.Shuffle();

            Model = all[0];
            for (int i = 0; i < count; i++) {
                Candidates.Add(all[i]);
            }
            Candidates.Shuffle();

            OnNewTable?.Invoke(Model, Candidates);
        }

        public bool Answer(CardT answer)
        {
            bool result = answer == Model;
            Messenger.Default.SendMessage(new GameplayEvents.Answered(result));

            if (!result) {
                Messenger.Default.SendMessage(new GameplayEvents.SubtractHP());
            }

            return result;
        }

        public bool Answer(int answer)
        {
            return Answer(Candidates[answer]);
        }
    }
}
