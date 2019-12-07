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
        private Random random = new Random();

        public ReactionChooserCore(List<CardT> allCards)
        {
            all = allCards;
        }

        public void NewTable(int count)
        {
            candidates.Clear();
            Shuffle(all);

            model = all[0];
            for (int i = 0; i < count; i++) {
                candidates.Add(all[i]);
            }
            Shuffle(candidates);

            OnNewTable?.Invoke(model, candidates);
        }

        public void Shuffle<T>(IList<T> list)
        {
            int n = list.Count;
            while (n > 1) {
                n--;
                int k = random.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }
    }
}