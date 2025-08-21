using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Common;
using System.Data.SQLite;
using System.Data.SQLite;
using System.IO;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static System.Net.WebRequestMethods;
using File = System.IO.File;

namespace Project_screenshot_ai
{
    internal class DatabaseHelper
    {
        private readonly string _connectionString;

        public DatabaseHelper(string dbPath , string folderPath)
        {
            _connectionString = $"Data Source={dbPath};Version=3;";

            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            if (!File.Exists(dbPath))
            {
                SQLiteConnection.CreateFile(dbPath);
            }
        }

        public void CreateTable()
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {            
                    connection.Open();            
                    string query = @"
                CREATE TABLE IF NOT EXISTS Questions (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Question TEXT NOT NULL,
                    Answer TEXT NOT NULL
                );";

                using (var command = new SQLiteCommand(query, connection))
                {
                    command.ExecuteNonQuery();
                }
            }

        }

        public void CreateFTS5table()
        {
            using (var conn = new SQLiteConnection(_connectionString))
            {
                conn.Open();
                conn.EnableExtensions(true);
                conn.LoadExtension("SQLite.Interop.dll", "sqlite3_fts5_init");
                var createCmd = conn.CreateCommand();
                createCmd.CommandText = @"
                CREATE VIRTUAL TABLE IF NOT EXISTS qa_search
                USING fts5(Question , Answer, Tokenize = 'unicode61');";
                createCmd.ExecuteNonQuery();

                var countCmd = conn.CreateCommand();
                countCmd.CommandText = "SELECT COUNT(*) FROM qa_search;";
                long rowCount = (long)(countCmd.ExecuteScalar() ?? 0);

                if (rowCount == 0)
                {
                    var copyCmd = conn.CreateCommand();
                    copyCmd.CommandText = "INSERT INTO qa_search (Question, Answer) SELECT Question, Answer FROM Questions;";
                    copyCmd.ExecuteNonQuery();
                }            
            }
        }

        public List<QA> SearchForMatchInFTS5(string[] matches, string db_name)
        {
            var result = new List<QA>();
            if (matches == null || matches.Length == 0)
                return result;

            string matchQuery = string.Join(" OR ", matches.Select(m => $"\"{m}\""));

            using(var conn = new SQLiteConnection(_connectionString))
            {
                conn.Open();
                conn.EnableExtensions(true);
                conn.LoadExtension("SQLite.Interop.dll", "sqlite3_fts5_init");
                matchQuery.Replace(@"\" , "");

                var searchCmd = conn.CreateCommand();
                searchCmd.CommandText = $"SELECT Question, Answer FROM {db_name} WHERE Question MATCH $term;";
                searchCmd.Parameters.AddWithValue("$term" , matchQuery);

                using (var reader = searchCmd.ExecuteReader()) {
                    while (reader.Read()) {
                        result.Add(new QA
                        {
                            Question = reader.GetString(0),
                            Answer = reader.GetString(1)
                        });
                    }
                }
            }
            return result;
        }


        public void InsertQuestions(List<QA> questions)
        { 
            using (var connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();

                string countSql = "SELECT COUNT(*) FROM Questions;";
                using (var countCmd = new SQLiteCommand(countSql, connection))
                {
                    long rowCount = (long)countCmd.ExecuteScalar();
                    if (rowCount > 0)
                        return;
                }

                foreach (var qa in questions)
                {
                    string query = @"INSERT INTO Questions (Question, Answer) VALUES (@q,@a)";
                    using (var command = new SQLiteCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@q", qa.Question);
                        command.Parameters.AddWithValue("@a", qa.Answer);
                        command.ExecuteNonQuery();
                    }
                }
            }
        }
}

}
