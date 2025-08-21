using System.Windows;
using BlackjackApp.helpers;

namespace BlackjackApp
{
    public partial class MainWindow : Window
    {
        private bool isFullscreen = true;
        private string currentUsername;
        private int chips;

        public MainWindow(string username)
        {
            InitializeComponent();
            currentUsername = username;
            chips = DatabaseHelper.GetChips(currentUsername, 1000);
            WelcomeTextBlock.Text = $"Welcome, {currentUsername}!";
            ChipsTextBlock.Text = $"Chips: {chips}";
        }

        private void StartGame_Click(object sender, RoutedEventArgs e)
        {
            var gameWindow = new GameWindow(currentUsername);
            gameWindow.Show();
            Close();
        }

        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            var login = new LoginWindow();
            login.Show();
            Close();
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
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
    }
}
