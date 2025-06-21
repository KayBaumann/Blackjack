using System.Windows;

namespace BlackjackApp
{
    public partial class MainWindow : Window
    {
        private bool isFullscreen = true;
        private string currentUsername;
        private int chips = 1000;

        public MainWindow(string username)
        {
            InitializeComponent();
            currentUsername = username;
            WelcomeTextBlock.Text = $"Welcome, {currentUsername}!";
            ChipsTextBlock.Text = $"Chips: {chips}";
        }

        private void StartGame_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Game will start...");
            // Game-Logik 
        }

        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            LoginWindow login = new LoginWindow();
            login.Show();
            this.Close();
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
