﻿using BlackjackApp.classes;
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
            UpdateWelcomeText();

            EndRoundUI();
            UpdateHandText();
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
            SurrenderButton.Visibility = Visibility.Collapsed;
            SplitButton.Visibility = Visibility.Collapsed;
        }

        private void ShowButtons()
        {
            HitButton.Visibility = Visibility.Visible;
            StandButton.Visibility = Visibility.Visible;
            DoubleDownButton.Visibility = game.SplitPerformed ? Visibility.Collapsed : Visibility.Visible;
            SurrenderButton.Visibility = game.PlayerActed ? Visibility.Collapsed : Visibility.Visible;

            if (game.CanSplit())
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
        }

        private void UpdateUI()
        {
            DealerHandTextBlock.Text = string.Join(", ", game.Dealer.Hand.Cards.Select(c => c.ToString())) + $" (Score: {game.Dealer.Hand.GetScore()})";
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
            this.Close();
        }
    }
}
