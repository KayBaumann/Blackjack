namespace BlackjackApp
{
    public class BlackjackGame
    {
        private Deck deck;
        public Player Player { get; private set; }
        public Player Dealer { get; private set; }

        public BlackjackGame(string playerName)
        {
            deck = new Deck();
            Player = new Player(playerName);
            Dealer = new Player("Dealer");
        }

        public void StartGame()
        {
            Player.Hand.AddCard(deck.DrawCard());
            Dealer.Hand.AddCard(deck.DrawCard());
            Player.Hand.AddCard(deck.DrawCard());
            Dealer.Hand.AddCard(deck.DrawCard());
        }

        public void PlayerHit()
        {
            Player.Hand.AddCard(deck.DrawCard());
        }

        public void DealerTurn()
        {
            while (Dealer.Hand.GetScore() < 17)
            {
                Dealer.Hand.AddCard(deck.DrawCard());
            }
        }

        public string GetResult()
        {
            if (Player.Hand.IsBust())
                return "Player Busts! Dealer wins!";
            if (Dealer.Hand.IsBust())
                return "Dealer Busts! Player wins!";
            if (Player.Hand.GetScore() > Dealer.Hand.GetScore())
                return "Player wins!";
            if (Player.Hand.GetScore() < Dealer.Hand.GetScore())
                return "Dealer wins!";
            return "Draw!";
        }
    }
}
