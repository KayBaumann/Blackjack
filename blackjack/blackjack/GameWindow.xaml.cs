using System.Linq;
using System.Windows;

namespace BlackjackApp
{
    public partial class GameWindow : Window
    {
        private bool isFullscreen = true;
        private BlackjackGame game;
        private string currentUsername;

        public GameWindow(string username)
        {
            InitializeComponent();
            currentUsername = username;
            StartNewGame();
        }

        private void StartNewGame()
        {
            game = new BlackjackGame(currentUsername);
            game.StartGame();
            UpdateUI();
            GameStatusTextBlock.Text = "";

            HitButton.Visibility = Visibility.Visible;
            StandButton.Visibility = Visibility.Visible;
            NewGameButton.Visibility = Visibility.Collapsed;
        }

        private void UpdateUI()
        {
            DealerHandTextBlock.Text = string.Join(", ", game.Dealer.Hand.Cards.Select(c => c.ToString())) + $" (Score: {game.Dealer.Hand.GetScore()})";
            PlayerHandTextBlock.Text = string.Join(", ", game.Player.Hand.Cards.Select(c => c.ToString())) + $" (Score: {game.Player.Hand.GetScore()})";
        }

        private void Hit_Click(object sender, RoutedEventArgs e)
        {
            game.PlayerHit();
            UpdateUI();

            if (game.Player.Hand.IsBust())
            {
                GameStatusTextBlock.Text = game.GetResult();
                HitButton.Visibility = Visibility.Collapsed;
                StandButton.Visibility = Visibility.Collapsed;
                NewGameButton.Visibility = Visibility.Visible;
            }
        }

        private void Stand_Click(object sender, RoutedEventArgs e)
        {
            game.DealerTurn();
            UpdateUI();
            GameStatusTextBlock.Text = game.GetResult();

            HitButton.Visibility = Visibility.Collapsed;
            StandButton.Visibility = Visibility.Collapsed;
            NewGameButton.Visibility = Visibility.Visible;
        }

        private void NewGame_Click(object sender, RoutedEventArgs e)
        {
            StartNewGame();
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
