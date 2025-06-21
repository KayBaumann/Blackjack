using System;
using System.Data.SQLite;
using System.IO;

namespace BlackjackApp
{
    public static class DatabaseHelper
    {
        private static string dbFile = "blackjack.db";

        public static void InitializeDatabase()
        {
            if (!File.Exists(dbFile))
            {
                SQLiteConnection.CreateFile(dbFile);

                using (var connection = new SQLiteConnection($"Data Source={dbFile};Version=3;"))
                {
                    connection.Open();

                    string createTableQuery = @"CREATE TABLE Users (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        Username TEXT NOT NULL UNIQUE,
                        PasswordHash TEXT NOT NULL
                    );";

                    using (var command = new SQLiteCommand(createTableQuery, connection))
                    {
                        command.ExecuteNonQuery();
                    }
                }
            }
        }

        public static SQLiteConnection GetConnection()
        {
            return new SQLiteConnection($"Data Source={dbFile};Version=3;");
        }
    }
}
