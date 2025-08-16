using BlackjackApp.classes;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace BlackjackApp
{
    public partial class GameWindow : Window
    {
        private bool isFullscreen = true;
        private BlackjackGame game;
        private string currentUsername;
        private int chips = 1000;
        private bool insuranceOffered = false;
        private int? currentBetPlaced = null;

        public GameWindow(string username)
        {
            InitializeComponent();
            currentUsername = username;
            UpdateWelcomeText();
        }

        private void UpdateWelcomeText()
        {
            if (currentBetPlaced.HasValue)
                WelcomeTextBlock.Text = $"Welcome, {currentUsername}\nChips: {chips} – Bet: {currentBetPlaced.Value}";
            else
                WelcomeTextBlock.Text = $"Welcome, {currentUsername}\nChips: {chips}";
        }

        private void PlaceBet_Click(object sender, RoutedEventArgs e)
        {
            if (!int.TryParse(BetInput.Text, out int bet) || bet <= 0)
            {
                GameStatusTextBlock.Text = "Invalid bet!";
                return;
            }

            if (bet > chips)
            {
                GameStatusTextBlock.Text = "Not enough chips!";
                return;
            }

            currentBetPlaced = bet;
            BetPanel.Visibility = Visibility.Collapsed;
            BackToMenuButton.Visibility = Visibility.Collapsed;
            UpdateWelcomeText();

            game = new BlackjackGame(currentUsername, bet);
            game.StartGame();
            UpdateUI();

            if (game.OfferInsurance())
            {
                InsuranceButton.Visibility = Visibility.Visible;
                insuranceOffered = true;
            }
            else
            {
                InsuranceButton.Visibility = Visibility.Collapsed;
                insuranceOffered = false;
            }

            if (game.Player.Hand.IsBlackjack())
            {
                game.RevealDealer();
                game.DealerTurn();
                UpdateUI();

                if (game.Dealer.Hand.IsBlackjack())
                {
                    GameStatusTextBlock.Text = "Push – Both have Blackjack!";
                }
                else
                {
                    int payout = game.CalculateBlackjackPayout();
                    chips += payout;
                    GameStatusTextBlock.Text = $"Blackjack! You win {payout} chips.";
                }

                EndRoundUI();
            }
            else
            {
                GameStatusTextBlock.Text = "";
                ShowButtons();
            }

            UpdateWelcomeText();
        }

        private void Hit_Click(object sender, RoutedEventArgs e)
        {
            game.PlayerHit();
            UpdateUI();
            ShowButtons();

            if (game.Player.Hand.IsBust())
            {
                if (game.NextHand())
                {
                    GameStatusTextBlock.Text = $"Hand busted. Now playing hand {game.Player.ActiveHandIndex + 1}";
                    UpdateUI();
                    return;
                }

                GameStatusTextBlock.Text = "Busted!";
                chips -= game.Bet;
                game.RevealDealer();
                UpdateUI();
                EndRoundUI();
                UpdateWelcomeText();
            }
        }

        private void Stand_Click(object sender, RoutedEventArgs e)
        {
            if (game.NextHand())
            {
                GameStatusTextBlock.Text = $"Now playing hand {game.Player.ActiveHandIndex + 1}";
                UpdateUI();
                ShowButtons();
                return;
            }

            game.RevealDealer();
            game.DealerTurn();
            UpdateUI();

            int result = game.CalculatePayout();

            if (game.InsuranceTaken)
            {
                GameStatusTextBlock.Text += game.InsuranceWon ? "Insurance won!\n" : "Insurance lost.\n";
            }

            if (result > 0)
                GameStatusTextBlock.Text += $"You win {result} chips!";
            else if (result < 0)
                GameStatusTextBlock.Text += $"You lose {-result} chips!";
            else
                GameStatusTextBlock.Text += "Draw!";

            chips += result;
            EndRoundUI();
            UpdateWelcomeText();
        }

        private void DoubleDown_Click(object sender, RoutedEventArgs e)
        {
            if (chips < game.Bet)
            {
                GameStatusTextBlock.Text = "Not enough chips to double down.";
                return;
            }

            game.PlayerDouble();
            UpdateUI();
            ShowButtons();

            if (game.Player.Hand.IsBust())
            {
                GameStatusTextBlock.Text = "Busted after Double Down!";
                chips -= game.Bet;
                game.RevealDealer();
                UpdateUI();
                EndRoundUI();
                UpdateWelcomeText();
            }
            else
            {
                Stand_Click(sender, e);
            }
        }

        private void SplitButton_Click(object sender, RoutedEventArgs e)
        {
            game.PerformSplit();
            GameStatusTextBlock.Text = "Hand split. Playing first hand.";
            UpdateUI();
            ShowButtons();
        }

        private void InsuranceButton_Click(object sender, RoutedEventArgs e)
        {
            game.TakeInsurance();
            InsuranceButton.Visibility = Visibility.Collapsed;
            GameStatusTextBlock.Text = "Insurance taken.";
        }

        private void SurrenderButton_Click(object sender, RoutedEventArgs e)
        {
            game.Surrender();
            GameStatusTextBlock.Text = "You surrendered. Half your bet is lost.";
            chips += game.CalculatePayout();
            game.RevealDealer();
            UpdateHandText();
            EndRoundUI();
            UpdateWelcomeText();
        }

        private void NewGame_Click(object sender, RoutedEventArgs e)
        {
            ResetUI();
        }

        private void ResetUI()
        {
            currentBetPlaced = null;
            BetPanel.Visibility = Visibility.Visible;
            BackToMenuButton.Visibility = Visibility.Visible;

            BetInput.Text = "100";
            GameStatusTextBlock.Text = "";
            DealerHandTextBlock.Text = "";
            PlayerHandTextBlock.Text = "";

            HitButton.Visibility = Visibility.Collapsed;
            StandButton.Visibility = Visibility.Collapsed;
            DoubleDownButton.Visibility = Visibility.Collapsed;
            NewGameButton.Visibility = Visibility.Collapsed;
            InsuranceButton.Visibility = Visibility.Collapsed;
            SurrenderButton.Visibility = Visibility.Collapsed;
            SplitButton.Visibility = Visibility.Collapsed;

            UpdateWelcomeText();
        }

        private void ShowButtons()
        {
            HitButton.Visibility = Visibility.Visible;
            StandButton.Visibility = Visibility.Visible;

            var twoCards = game.Player.Hand.Cards.Count == 2;
            var allowOpeningActions = !game.PlayerActed && !game.SplitPerformed && twoCards;

            DoubleDownButton.Visibility = allowOpeningActions ? Visibility.Visible : Visibility.Collapsed;
            SurrenderButton.Visibility = allowOpeningActions ? Visibility.Visible : Visibility.Collapsed;

            if (!game.PlayerActed && game.CanSplit())
                SplitButton.Visibility = Visibility.Visible;
            else
                SplitButton.Visibility = Visibility.Collapsed;
        }

        private void EndRoundUI()
        {
            HitButton.Visibility = Visibility.Collapsed;
            StandButton.Visibility = Visibility.Collapsed;
            DoubleDownButton.Visibility = Visibility.Collapsed;
            NewGameButton.Visibility = Visibility.Visible;
            InsuranceButton.Visibility = Visibility.Collapsed;
            SurrenderButton.Visibility = Visibility.Collapsed;
            SplitButton.Visibility = Visibility.Collapsed;
            BackToMenuButton.Visibility = Visibility.Visible;
        }

        private void UpdateUI()
        {
            var dealerCards = game.Dealer.Hand.Cards;

            if (game.RevealDealerHole)
                DealerHandTextBlock.Text = string.Join(", ", dealerCards.Select(c => c.ToString())) + $" (Score: {game.Dealer.Hand.GetScore()})";
            else
                DealerHandTextBlock.Text = $"{dealerCards[0]}, [Hidden]";

            PlayerHandTextBlock.Text = string.Join(", ", game.Player.Hand.Cards.Select(c => c.ToString())) + $" (Score: {game.Player.Hand.GetScore()})";
        }

        private void UpdateHandText()
        {
            UpdateUI();
        }

        private void ToggleFullscreen_Click(object sender, RoutedEventArgs e)
        {
            if (isFullscreen)
            {
                WindowStyle = WindowStyle.SingleBorderWindow;
                WindowState = WindowState.Normal;
                isFullscreen = false;
            }
            else
            {
                WindowStyle = WindowStyle.None;
                WindowState = WindowState.Maximized;
                isFullscreen = true;
            }
        }

        private void BackToMenu_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow(currentUsername);
            mainWindow.Show();
            Close();
        }
    }
}
