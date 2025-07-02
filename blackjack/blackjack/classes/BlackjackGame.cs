using System.Linq;
using System.Collections.Generic;

namespace BlackjackApp.classes
{
    public class BlackjackGame
    {
        private Deck deck;
        public Player Player { get; private set; }
        public Player Dealer { get; private set; }
        public int Bet { get; private set; }

        public bool PlayerDoubleDown { get; private set; }
        public bool PlayerSurrendered { get; private set; }

        public bool InsuranceOffered { get; private set; }
        public bool InsuranceTaken { get; private set; }
        public bool InsuranceWon { get; private set; }

        public bool SplitPerformed { get; private set; }
        public bool PlayerActed { get; private set; }

        public BlackjackGame(string playerName, int bet)
        {
            deck = new Deck();
            Player = new Player(playerName);
            Dealer = new Player("Dealer");
            Bet = bet;
            PlayerDoubleDown = false;
            PlayerSurrendered = false;
            InsuranceOffered = false;
            InsuranceTaken = false;
            InsuranceWon = false;
            SplitPerformed = false;
            PlayerActed = false;
        }

        public void StartGame()
        {
            Player.Hand.AddCard(deck.DrawCard());
            Dealer.Hand.AddCard(deck.DrawCard());
            Player.Hand.AddCard(deck.DrawCard());
            Dealer.Hand.AddCard(deck.DrawCard());

            OfferInsurance();
        }

        public bool IsPlayerBlackjack()
        {
            return Player.Hand.IsBlackjack() && !SplitPerformed;
        }

        public bool IsDealerBlackjack() => Dealer.Hand.IsBlackjack();

        public void PlayerHit()
        {
            RegisterPlayerAction();
            Player.Hand.AddCard(deck.DrawCard());
        }

        public void PlayerDouble()
        {
            if (SplitPerformed) return;
            if (!PlayerDoubleDown)
            {
                RegisterPlayerAction();
                PlayerDoubleDown = true;
                Bet *= 2;
                Player.Hand.AddCard(deck.DrawCard());
            }
        }

        public void DealerTurn()
        {
            while (Dealer.Hand.GetScore() < 17 || (Dealer.Hand.GetScore() == 17 && Dealer.Hand.HasSoft17()))
            {
                Dealer.Hand.AddCard(deck.DrawCard());
            }
        }

        public void Surrender()
        {
            PlayerSurrendered = true;
        }

        public bool OfferInsurance()
        {
            InsuranceOffered = Dealer.Hand.Cards.First().Rank == Rank.Ace;
            return InsuranceOffered;
        }

        public void TakeInsurance()
        {
            if (InsuranceOffered)
            {
                InsuranceTaken = true;
            }
        }

        public bool CanSplit()
        {
            var cards = Player.Hand.Cards;
            return cards.Count == 2 && cards[0].Rank == cards[1].Rank;
        }

        public void PerformSplit()
        {
            RegisterPlayerAction();

            var original = Player.Hand;
            var splitCard = original.Cards[1];
            original.Cards.RemoveAt(1);

            var newHand = new Hand();
            newHand.AddCard(splitCard);

            original.AddCard(deck.DrawCard());
            newHand.AddCard(deck.DrawCard());

            Player.Hands = new List<Hand> { original, newHand };
            Player.ActiveHandIndex = 0;
            SplitPerformed = true;
        }

        public bool NextHand()
        {
            if (Player.ActiveHandIndex + 1 < Player.Hands.Count)
            {
                Player.ActiveHandIndex++;
                return true;
            }
            return false;
        }

        public int CalculatePayout()
        {
            if (PlayerSurrendered)
                return -Bet / 2;

            int payout = 0;

            if (InsuranceTaken)
            {
                if (IsDealerBlackjack())
                {
                    InsuranceWon = true;
                    payout += (int)(Bet * 0.5 * 2);
                }
                else
                {
                    payout -= (int)(Bet * 0.5);
                }
            }

            if (IsPlayerBlackjack() && !IsDealerBlackjack())
            {
                payout += (int)(Bet * 1.5);
            }
            else if (IsPlayerBlackjack() && IsDealerBlackjack())
            {
                payout += 0;
            }
            else if (IsDealerBlackjack() && !IsPlayerBlackjack())
            {
                payout -= Bet;
            }
            else if (Player.Hand.IsBust())
            {
                payout -= Bet;
            }
            else if (Dealer.Hand.IsBust())
            {
                payout += Bet;
            }
            else if (Player.Hand.GetScore() > Dealer.Hand.GetScore())
            {
                payout += Bet;
            }
            else if (Player.Hand.GetScore() < Dealer.Hand.GetScore())
            {
                payout -= Bet;
            }

            return payout;
        }

        public int CalculateBlackjackPayout()
        {
            return (int)(Bet * 1.5);
        }

        public void RegisterPlayerAction()
        {
            PlayerActed = true;
        }
    }
}
