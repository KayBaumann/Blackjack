using System;
using System.Data;
using System.Data.SQLite;

namespace BlackjackApp.helpers
{
    public static class DatabaseHelper
    {
        private static readonly string Cs = "Data Source=blackjack.db;Version=3;";

        public static SQLiteConnection GetConnection() => new SQLiteConnection(Cs);

        public static void EnsureChipsColumn()
        {
            using var con = new SQLiteConnection(Cs);
            con.Open();
            using var cmd = new SQLiteCommand("PRAGMA table_info(Users);", con);
            using var rd = cmd.ExecuteReader();
            var has = false;
            while (rd.Read())
                if (string.Equals(rd["name"]?.ToString(), "Chips", StringComparison.OrdinalIgnoreCase))
                    has = true;
            if (!has)
                new SQLiteCommand("ALTER TABLE Users ADD COLUMN Chips INTEGER NOT NULL DEFAULT 1000;", con).ExecuteNonQuery();
        }

        public static void EnsureIndexes()
        {
            using var con = new SQLiteConnection(Cs);
            con.Open();
            new SQLiteCommand("CREATE INDEX IF NOT EXISTS idx_users_username ON Users(Username);", con).ExecuteNonQuery();
        }

        public static int GetChips(string username, int @default = 1000)
        {
            using var con = new SQLiteConnection(Cs);
            con.Open();
            using var cmd = new SQLiteCommand("SELECT Chips FROM Users WHERE Username=@u LIMIT 1;", con);
            cmd.Parameters.AddWithValue("@u", username);
            var obj = cmd.ExecuteScalar();
            if (obj == null || obj == DBNull.Value) return @default;
            return Convert.ToInt32(obj);
        }

        public static void SetChips(string username, int chips)
        {
            using var con = new SQLiteConnection(Cs);
            con.Open();
            using var cmd = new SQLiteCommand("UPDATE Users SET Chips=@c WHERE Username=@u;", con);
            cmd.Parameters.AddWithValue("@c", chips);
            cmd.Parameters.AddWithValue("@u", username);
            cmd.ExecuteNonQuery();
        }
    }
}
