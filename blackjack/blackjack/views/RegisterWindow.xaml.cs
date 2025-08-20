using System;
using System.Windows;
using System.Data.SQLite;
using BlackjackApp.helpers;

namespace BlackjackApp
{
    public partial class RegisterWindow : Window
    {
        public RegisterWindow()
        {
            InitializeComponent();
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void Register_Click(object sender, RoutedEventArgs e)
        {
            string username = UsernameTextBox.Text.Trim();
            string password = PasswordBox.Password;
            string confirmPassword = ConfirmPasswordBox.Password;

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(confirmPassword))
            {
                MessageBox.Show("Please fill all fields.");
                return;
            }

            if (password != confirmPassword)
            {
                MessageBox.Show("Passwords do not match.");
                return;
            }

            string hashedPassword = PasswordHelper.HashPassword(password);

            using (var connection = DatabaseHelper.GetConnection())
            {
                connection.Open();
                string query = "INSERT INTO Users (Username, PasswordHash) VALUES (@username, @passwordHash)";

                using (var command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@username", username);
                    command.Parameters.AddWithValue("@passwordHash", hashedPassword);
                    try
                    {
                        command.ExecuteNonQuery();

                        // Chips initialisieren (Default 1000)
                        DatabaseHelper.EnsureChipsColumn();
                        DatabaseHelper.SetChips(username, 1000);

                        MessageBox.Show("Registration successful!");

                        LoginWindow loginWindow = new LoginWindow();
                        loginWindow.Show();
                        this.Close();
                    }
                    catch (SQLiteException ex)
                    {
                        if (ex.ResultCode == SQLiteErrorCode.Constraint)
                        {
                            MessageBox.Show("Username already exists.");
                        }
                        else
                        {
                            MessageBox.Show("Error: " + ex.Message);
                        }
                    }
                }
            }
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            LoginWindow loginWindow = new LoginWindow();
            loginWindow.Show();
            this.Close();
        }
    }
}
