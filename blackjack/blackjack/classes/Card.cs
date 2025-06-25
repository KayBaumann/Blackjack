namespace BlackjackApp
{
    public enum Suit { Hearts, Diamonds, Clubs, Spades }
    public enum Rank { Two = 2, Three, Four, Five, Six, Seven, Eight, Nine, Ten, Jack, Queen, King, Ace }

    public class Card
    {
        public Suit Suit { get; }
        public Rank Rank { get; }

        public Card(Suit suit, Rank rank)
        {
            Suit = suit;
            Rank = rank;
        }

        public int GetValue()
        {
            if (Rank >= Rank.Two && Rank <= Rank.Ten)
                return (int)Rank;
            if (Rank == Rank.Jack || Rank == Rank.Queen || Rank == Rank.King)
                return 10;
            return 11;
        }

        public override string ToString()
        {
            return $"{Rank} of {Suit}";
        }
    }
}
