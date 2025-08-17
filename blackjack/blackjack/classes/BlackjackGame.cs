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
        public bool RevealDealerHole { get; private set; }

        public bool StrictSplitAcesRule { get; set; } = true;
        private bool SplitAces;
        private bool SplitBetCharged;

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
            RevealDealerHole = false;
        }

        public void StartGame()
        {
            Player.Hand.AddCard(deck.DrawCard());
            Dealer.Hand.AddCard(deck.DrawCard());
            Player.Hand.AddCard(deck.DrawCard());
            Dealer.Hand.AddCard(deck.DrawCard());
            OfferInsurance();
            RevealDealerHole = IsDealerBlackjack();
        }

        public void RevealDealer()
        {
            RevealDealerHole = true;
        }

        public bool IsPlayerBlackjack()
        {
            return Player.Hand.IsBlackjack() && !SplitPerformed;
        }

        public bool IsDealerBlackjack()
        {
            return Dealer.Hand.IsBlackjack();
        }

        public void PlayerHit()
        {
            if (SplitAces && StrictSplitAcesRule) return;
            PlayerActed = true;
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
            RevealDealerHole = true;
            while (true)
            {
                int score = Dealer.Hand.GetScore();
                if (score < 17) { Dealer.Hand.AddCard(deck.DrawCard()); continue; }
                if (score == 17 && Dealer.Hand.HasSoft17()) { Dealer.Hand.AddCard(deck.DrawCard()); continue; }
                break;
            }
        }

        public bool CanSurrender()
        {
            if (SplitPerformed) return false;
            if (IsDealerBlackjack()) return false;
            if (Player.Hand.Cards.Count != 2) return false;
            if (PlayerActed) return false;
            return true;
        }

        public void Surrender()
        {
            if (!CanSurrender()) return;
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

        public void PlayerStand()
        {
            PlayerActed = true;
        }

        public bool CanSplit()
        {
            var cards = Player.Hand.Cards;
            return cards.Count == 2 && cards[0].Rank == cards[1].Rank;
        }

        public void PerformSplit()
        {
            PlayerActed = true;
            var first = Player.Hand;
            var splitCard = first.Cards[1];
            first.Cards.RemoveAt(1);
            var second = new Hand();
            second.AddCard(splitCard);
            first.AddCard(deck.DrawCard());
            second.AddCard(deck.DrawCard());
            Player.Hands = new List<Hand> { first, second };
            Player.ActiveHandIndex = 0;
            SplitPerformed = true;
            SplitAces = first.Cards[0].Rank == Rank.Ace && second.Cards[0].Rank == Rank.Ace;
            SplitBetCharged = false;
            if (SplitAces && StrictSplitAcesRule)
            {
                Player.ActiveHandIndex = 0;
            }
        }

        public int ConsumeSplitBet()
        {
            if (!SplitPerformed) return 0;
            if (SplitBetCharged) return 0;
            SplitBetCharged = true;
            return Bet;
        }

        public bool NextHand()
        {
            if (!SplitPerformed) return false;
            if (Player.ActiveHandIndex + 1 < Player.Hands.Count)
            {
                Player.ActiveHandIndex++;
                return true;
            }
            return false;
        }

        public int CalculatePayout()
        {
            if (PlayerSurrendered) return -Bet / 2;

            int payout = 0;

            if (InsuranceTaken)
            {
                payout -= Bet / 2;
                if (IsDealerBlackjack())
                {
                    InsuranceWon = true;
                    payout += Bet;
                }
            }

            if (IsDealerBlackjack() && !IsPlayerBlackjack())
            {
                if (!SplitPerformed) return payout - Bet;
                foreach (var _ in Player.Hands) payout -= Bet;
                return payout;
            }

            if (!RevealDealerHole) RevealDealer();
            DealerTurn();

            if (!SplitPerformed)
            {
                payout += SettleHand(Player.Hand);
                return payout;
            }

            foreach (var hand in Player.Hands)
            {
                payout += SettleHand(hand);
            }

            return payout;
        }

        private int SettleHand(Hand hand)
        {
            if (hand.IsBust()) return -Bet;
            if (Dealer.Hand.IsBust()) return Bet;

            int ps = hand.GetScore();
            int ds = Dealer.Hand.GetScore();

            if (hand.IsBlackjack() && !SplitPerformed) return (int)(Bet * 1.5);
            if (ps > ds) return Bet;
            if (ps < ds) return -Bet;
            return 0;
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
