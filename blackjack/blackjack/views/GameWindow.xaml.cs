using BlackjackApp.classes;
using System.Linq;
using System.Windows;

namespace BlackjackApp
{
    public partial class GameWindow : Window
    {
        private bool isFullscreen = true;
        private BlackjackGame game;
        private string currentUsername;
        private int chips = 1000;
        private bool insuranceOffered = false;

        public GameWindow(string username)
        {
            InitializeComponent();
            currentUsername = username;
            UpdateWelcomeText();
        }

        private void UpdateWelcomeText()
        {
            WelcomeTextBlock.Text = $"Welcome, {currentUsername} – Chips: {chips}";
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
            BetInput.IsEnabled = false;
        }

        private void Hit_Click(object sender, RoutedEventArgs e)
        {
            game.PlayerHit();
            UpdateUI();

            if (game.Player.Hand.IsBust())
            {
                GameStatusTextBlock.Text = "Busted!";
                chips -= game.Bet;
                EndRoundUI();
                UpdateWelcomeText();
            }
        }

        private void Stand_Click(object sender, RoutedEventArgs e)
        {
            game.DealerTurn();
            UpdateUI();

            int result = game.CalculatePayout();

            if (game.InsuranceTaken)
            {
                if (game.InsuranceWon)
                    GameStatusTextBlock.Text += "Insurance won!\n";
                else
                    GameStatusTextBlock.Text += "Insurance lost.\n";
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

            if (game.Player.Hand.IsBust())
            {
                GameStatusTextBlock.Text = "Busted after Double Down!";
                chips -= game.Bet;
            }
            else
            {
                Stand_Click(sender, e); // Auto-Stand
            }

            UpdateWelcomeText();
        }

        private void InsuranceButton_Click(object sender, RoutedEventArgs e)
        {
            game.TakeInsurance();
            InsuranceButton.Visibility = Visibility.Collapsed;
            GameStatusTextBlock.Text = "Insurance taken.";
        }

        private void NewGame_Click(object sender, RoutedEventArgs e)
        {
            ResetUI();
        }

        private void ResetUI()
        {
            BetInput.Text = "100";
            BetInput.IsEnabled = true;
            GameStatusTextBlock.Text = "";
            DealerHandTextBlock.Text = "";
            PlayerHandTextBlock.Text = "";
            HitButton.Visibility = Visibility.Collapsed;
            StandButton.Visibility = Visibility.Collapsed;
            DoubleDownButton.Visibility = Visibility.Collapsed;
            NewGameButton.Visibility = Visibility.Collapsed;
            InsuranceButton.Visibility = Visibility.Collapsed;
        }

        private void ShowButtons()
        {
            HitButton.Visibility = Visibility.Visible;
            StandButton.Visibility = Visibility.Visible;
            DoubleDownButton.Visibility = Visibility.Visible;
        }

        private void EndRoundUI()
        {
            HitButton.Visibility = Visibility.Collapsed;
            StandButton.Visibility = Visibility.Collapsed;
            DoubleDownButton.Visibility = Visibility.Collapsed;
            NewGameButton.Visibility = Visibility.Visible;
            InsuranceButton.Visibility = Visibility.Collapsed;
        }

        private void UpdateUI()
        {
            DealerHandTextBlock.Text = string.Join(", ", game.Dealer.Hand.Cards.Select(c => c.ToString())) + $" (Score: {game.Dealer.Hand.GetScore()})";
            PlayerHandTextBlock.Text = string.Join(", ", game.Player.Hand.Cards.Select(c => c.ToString())) + $" (Score: {game.Player.Hand.GetScore()})";
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
            this.Close();
        }
    }
}
