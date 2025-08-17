using System.Collections.Generic;
using System.Linq;

namespace BlackjackApp
{
    public class Hand
    {
        public List<Card> Cards { get; } = new List<Card>();

        public void AddCard(Card card)
        {
            Cards.Add(card);
        }

        public int GetScore()
        {
            int total = Cards.Sum(c => c.GetValue());
            int aces = Cards.Count(c => c.Rank == Rank.Ace);

            while (total > 21 && aces > 0)
            {
                total -= 10;
                aces--;
            }
            return total;
        }

        public bool HasSoft17()
        {
            int nonAces = Cards.Where(c => c.Rank != Rank.Ace).Sum(c => c.GetValue());
            int aces = Cards.Count(c => c.Rank == Rank.Ace);
            if (aces == 0) return false;
            return nonAces + 11 + (aces - 1) == 17;
        }

        public bool IsBust() => GetScore() > 21;
        public bool IsBlackjack() => Cards.Count == 2 && GetScore() == 21;
    }
}
