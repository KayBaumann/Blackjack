using System.Windows;
using System.Data.SQLite;
using BlackjackApp.helpers;

namespace BlackjackApp
{
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void Login_Click(object sender, RoutedEventArgs e)
        {
            string username = UsernameTextBox.Text.Trim();
            string password = PasswordBox.Password;

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Please enter both username and password.");
                return;
            }

            string hashedPassword = PasswordHelper.HashPassword(password);

            using (var connection = DatabaseHelper.GetConnection())
            {
                connection.Open();
                const string query = "SELECT COUNT(*) FROM Users WHERE Username = @username AND PasswordHash = @passwordHash";

                using (var command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@username", username);
                    command.Parameters.AddWithValue("@passwordHash", hashedPassword);

                    var count = (long)command.ExecuteScalar();
                    if (count > 0)
                    {
                        MessageBox.Show("Login successful!");
                        MainWindow mainWindow = new MainWindow(username);
                        mainWindow.Show();
                        Close();
                    }
                    else
                    {
                        MessageBox.Show("Invalid username or password.");
                    }
                }
            }
        }

        private void Register_Click(object sender, RoutedEventArgs e)
        {
            RegisterWindow registerWindow = new RegisterWindow();
            registerWindow.Show();
            Close();
        }
    }
}
