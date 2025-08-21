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
          




            Application.Run(new Form1());

        }

        private static void InitiateStreamWriter()
        {
            string path = Path.Combine(Application.StartupPath,"Logs","logs.txt");
            var logFile = new StreamWriter(path) { AutoFlush = true };
            Console.SetOut(TextWriter.Synchronized(
                new StreamWriterMulti(Console.Out, logFile)
            ));
        }

        private static void InitiateDB()
        {
            string folderPath = Path.Combine(Application.StartupPath, "Data", "db");
            string dbPath = Path.Combine(folderPath, "stage_1.db");
            string jsonPath = Path.Combine(Application.StartupPath,"Data", "json", "stage_1.json");

            DatabaseHelper db = new DatabaseHelper(dbPath , folderPath);
            db.CreateTable();

            
            var questions = JsonHelper.LoadFromJson(jsonPath);

            db.InsertQuestions(questions);

            db.CreateFTS5table();

            Console.WriteLine("All questions inserted successfully!");
        }
    }
}
