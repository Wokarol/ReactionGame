using System;
using System.Collections;
using System.Collections.Generic;

using Wokarol.StateMachineSystem;

namespace Wokarol.GameplayCores
{
    public class ReactionChooserCore<CardT> where CardT : class
    {
        public delegate void TableCreated(CardT model, List<CardT> candidates);

        public event TableCreated OnNewTable;

        private List<CardT> all;

        // TableData
        private List<CardT> candidates = new List<CardT>();
        private CardT model;

        public ReactionChooserCore(List<CardT> allCards)
        {
            all = allCards;
        }

        public void NewTable(int count)
        {
            candidates.Clear();
            all.Shuffle();

            model = all[0];
            for (int i = 0; i < count; i++) {
                candidates.Add(all[i]);
            }
            candidates.Shuffle();

            OnNewTable?.Invoke(model, candidates);
        }

        public bool Answer(CardT answer)
        {
            return answer == model;
        }

        public bool Answer(int answer)
        {
            return candidates[answer] == model;
        }
    }
}
