using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Tesseract;
using System.IO;
using Microsoft.Data.Sqlite;
using Newtonsoft.Json;
using System.Data.SQLite;

namespace Project_screenshot_ai
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            InitiateStreamWriter();
            InitiateDB();

            string mydb = "myDataBase.db";

            Application.Run(new Form1(mydb));

        }

        private static void InitiateStreamWriter()
        {
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs", "logs.txt");
            var logFile = new StreamWriter(path) { AutoFlush = true };
            Console.SetOut(TextWriter.Synchronized(
                new StreamWriterMulti(Console.Out, logFile)
            ));
        }

        private static void InitiateDB()
        {
            string folderPath = Path.Combine(Application.StartupPath, "Data", "db");
            string dbPath = Path.Combine(folderPath, "myDataBase.db");
            string[] feed = setPathToJsonName(new string[] { "stage_1.json", "stage_2.json" });
            string jsonPath = CombineJsons(feed , dbPath);

            

            if (jsonPath == null || jsonPath == "")
                return;

            DatabaseHelper db = new DatabaseHelper(dbPath, folderPath);

            db.CreateTable();

            var questions = JsonHelper.LoadFromJson(jsonPath);

            db.InsertQuestions(questions);

            db.CreateFTS5table();

            Console.WriteLine("All questions inserted successfully!");
        }

        private static string CombineJsons(string[] jsons , string dbpath)
        {
            if (File.Exists(setCombinedJsonsFileNameAndPath()) && File.Exists(dbpath))
                return "";

            var allEntries = new List<QA>();

            foreach (var file in jsons)
            {
                if (File.Exists(file))
                {
                    var text = File.ReadAllText(file);
                    var list = JsonConvert.DeserializeObject<List<QA>>(text);

                    if (list != null && list.Count > 0) { allEntries.AddRange(list); }
                }
                else {
                    Console.WriteLine($"⚠ File not found: {file}");
                }
            }

            var unique = allEntries
                .GroupBy(q => q.Question.Trim().ToLower())
                .Select(g => g.First())
                .ToList();

            var json = JsonConvert.SerializeObject(unique, Formatting.Indented);
            var jsonName = setCombinedJsonsFileNameAndPath();
            File.WriteAllText(jsonName, json);

            Console.WriteLine($"Merged {allEntries.Count} entries -> {unique.Count} unique saved.");

            return jsonName;
        }
        private static string[] setPathToJsonName(string[] json)
        {
            for (int i = 0; i < json.Length; i++)
            {
                json[i] = Path.Combine(Application.StartupPath, "Data", "json", json[i]);
            }
            return json;
        }
        private static string setCombinedJsonsFileNameAndPath()
        {
            return Path.Combine(Application.StartupPath, "Data", "json" , "merged.json");
        }
    }
}
