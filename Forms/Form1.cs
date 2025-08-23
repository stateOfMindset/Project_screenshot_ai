using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Tesseract;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace Project_screenshot_ai
{
    public partial class Form1 : Form
    {
        private System.Windows.Forms.Timer screenshotTimer;
        private bool isCapturing = false;
        private Rectangle selectedRegion = Rectangle.Empty;
        private OverlayForm overlay;
        private Bitmap ScreenCaptureForDisplay;
        private Random rnd = new Random();

        private string[] targetWords = { "מילה", "ל-", "נרדפת" };
        internal string dbname { get; set; }

        Dictionary<string, string> synonymMap = new Dictionary<string, string>
{
        { "new york", "nyc" },
        { "machine learning", "ml" },
        { "artificial intelligence", "ai" }
};



        public Form1(string dbname)
        {
            InitializeComponent();
            screenshotTimer = new System.Windows.Forms.Timer();

            screenshotTimer.Interval = 1000;

            screenshotTimer.Tick += ScreenshotTimer_Tick;
            screenshotTimer.Tick += Capture_screenshot_Click;
            this.dbname = dbname;


        }

        private void ScreenshotTimer_Tick(object sender, EventArgs e)
        {

            ProcessScreenshot();


        }

        private void captureButton_Click(object sender, EventArgs e)
        {

            if (!isCapturing)
            {
                screenshotTimer.Start();
                isCapturing = true;
                captureButton.Text = "Stop Capturing";
            }
            else
            {
                screenshotTimer.Stop();
                isCapturing = false;
                captureButton.Text = "Start Capturing";
            }
        }



        public static Bitmap CaptureScreen()
        {
            Rectangle bounds = Screen.PrimaryScreen.Bounds;
            Bitmap screenshot = new Bitmap(bounds.Width, bounds.Height, PixelFormat.Format32bppArgb);
            using (Graphics g = Graphics.FromImage(screenshot))
            {
                g.CopyFromScreen(bounds.Left, bounds.Top, 0, 0, bounds.Size, CopyPixelOperation.SourceCopy);
            }
            return screenshot;
        }

        public static string ExtractHebrewText(Bitmap image, string tessDataPath)
        {
            try
            {
                using (var engine = new TesseractEngine(tessDataPath, "heb", EngineMode.Default))
                using (var pix = PixConverter.ToPix(image))
                using (var page = engine.Process(pix))
                {
                    return page.GetText();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return "ERROR";
        }

        public void ShowResult(string answer, string question)
        {
            this.Invoke((MethodInvoker)delegate
            {
                if (label_verify_question.Text == question)
                    return;

                if (answer.Trim().Equals("נכון"))
                    resultLabel.BackColor = Color.Lime;
                else if (answer.Trim().Equals("לא נכון"))
                    resultLabel.BackColor = Color.Red;
                else
                {
                    resultLabel.BackColor = Color.Purple;
                    resultLabel.ForeColor = Color.White;
                }

                Color randomColor = Color.FromArgb(rnd.Next(100, 256), rnd.Next(100, 256), rnd.Next(100, 256));
                label_verify_question.BackColor = randomColor;

                resultLabel.Text = answer;
                label_verify_question.Text = question;
            });
        }

        private void ProcessScreenshot()
        {
            string tessDataPath = Path.Combine(Application.StartupPath, "Data", "tessdata");
            Bitmap screenshot;

            if (selectedRegion != RectangleF.Empty)
                screenshot = CaptureRegion(selectedRegion);
            else
                screenshot = CaptureScreen();

            var sw = Stopwatch.StartNew();

            Bitmap scaled = ScaleBitmap(screenshot, (float)1.1);
            Bitmap processed = Preprocess(scaled);
            string text = ExtractHebrewText(processed, tessDataPath);
            string[] split = cleanString(text);


            List<string> lines = new List<string>();
            foreach (string s in split)
            {
                if (!string.IsNullOrWhiteSpace(s))
                {
                    lines.Add(s);
                }
            }

            string returnRes = string.Join(" ", lines);

            string[] answers = Tokenize(returnRes);
            if (!(answers.Length > 0))
                return;

            //Console.WriteLine("elapsted 1 : " + sw.ElapsedMilliseconds);

            string folderPath = Path.Combine(Application.StartupPath, "Data", "db");
            string dbPath = Path.Combine(folderPath, dbname);
            Console.WriteLine(dbPath);
            DatabaseHelper dh = new DatabaseHelper(dbPath, folderPath);
            List<QA> Possible_results = dh.SearchForMatchInFTS5(answers, "qa_search");

            if (Possible_results.Count == 0)
                return;


            string originalQuestion = "";

            foreach (string s in answers)
            {
                originalQuestion += s;
            }

            //Console.WriteLine("elapsted 2 : " + sw.ElapsedMilliseconds);

            float bestMatchingFactor = 0;
            int i = 0;
            int bestMatchingFactorIndex = 0;
            foreach (QA p in Possible_results)
            {
                float mf = LevinshtienDistance(originalQuestion, p.Question);
                if (mf > bestMatchingFactor)
                {
                    bestMatchingFactor = mf;
                    bestMatchingFactorIndex = i;
                }
                i++;
            }

            QA res = Possible_results[bestMatchingFactorIndex];

            ShowResult(res.Answer, res.Question);

            sw.Stop();
            Console.WriteLine($"elapsted 3 : {sw.ElapsedMilliseconds} ms");
            Console.WriteLine(Possible_results[bestMatchingFactorIndex].Question);



        }
        public string[] cleanString(string text)
        {
            char[] sp = { '\n', '\r' };

            string clean = Regex.Replace(text, "[\\[\\]<>]", "");
            clean = clean.Replace("\"", "");
            return clean.Split(sp);
        }

        public static Bitmap Preprocess(Bitmap input)
        {
            // 1. Convert to grayscale
            Bitmap gray = new Bitmap(input.Width, input.Height, PixelFormat.Format24bppRgb);
            using (Graphics g = Graphics.FromImage(gray))
            {
                var colorMatrix = new System.Drawing.Imaging.ColorMatrix(
                    new float[][]
                    {
                new float[] {0.3f, 0.3f, 0.3f, 0, 0},
                new float[] {0.59f, 0.59f, 0.59f, 0, 0},
                new float[] {0.11f, 0.11f, 0.11f, 0, 0},
                new float[] {0,    0,    0,    1, 0},
                new float[] {0,    0,    0,    0, 1}
                    });
                var attributes = new ImageAttributes();
                attributes.SetColorMatrix(colorMatrix);
                g.DrawImage(input, new Rectangle(0, 0, input.Width, input.Height), 0, 0, input.Width, input.Height, GraphicsUnit.Pixel, attributes);
            }

            bool isDark = IsImageDark(gray);

            for (int y = 0; y < gray.Height; y++)
            {
                for (int x = 0; x < gray.Width; x++)
                {
                    Color pixel = gray.GetPixel(x, y);
                    int brightness = (pixel.R + pixel.G + pixel.B) / 3;
                    Color newColor;

                    if (isDark)
                        newColor = (brightness > 128) ? Color.Black : Color.White;
                    else
                        newColor = (brightness > 128) ? Color.White : Color.Black;

                    gray.SetPixel(x, y, newColor);
                }
            }

            return gray;
        }

        private static bool IsImageDark(Bitmap image)
        {
            long totalBrightness = 0;
            for (int y = 0; y < image.Height; y++)
            {
                for (int x = 0; x < image.Width; x++)
                {
                    Color pixel = image.GetPixel(x, y);
                    int brightness = (pixel.R + pixel.G + pixel.B) / 3;
                    totalBrightness += brightness;
                }
            }

            long totalPixels = image.Width * image.Height;
            int averageBrightness = (int)(totalBrightness / totalPixels);

            return averageBrightness < 128;
        }


        private void selectRegionButton_Click(object sender, EventArgs e)
        {
            if (overlay == null || overlay.IsDisposed)
            {
                overlay = new OverlayForm();
                overlay.SelectionFinalized += Overlay_SelectionFinalized;
                overlay.Show();
            }
        }

        private void Overlay_SelectionFinalized(object sender, EventArgs e)
        {
            if (overlay.SelectionRectangle.Width > 0 && overlay.SelectionRectangle.Height > 0)
            {
                selectedRegion = overlay.SelectionRectangle;
                Console.WriteLine($"Region selected: {selectedRegion}");
            }
            else
                Console.WriteLine("No region selected.");
        }



        public static Bitmap CaptureRegion(Rectangle region)
        {
            Bitmap bmp = new Bitmap(region.Width, region.Height, PixelFormat.Format32bppArgb);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.CopyFromScreen(region.Left, region.Top, 0, 0, region.Size, CopyPixelOperation.SourceCopy);
            }

            return bmp;

        }

        private void Capture_screenshot_Click(object sender, EventArgs e)
        {

            if (selectedRegion == Rectangle.Empty)
                return;

            this.Invoke((MethodInvoker)delegate
            {

                ScreenCaptureForDisplay = CaptureRegion(selectedRegion);
                pictureBox1.SizeMode = PictureBoxSizeMode.AutoSize;
                pictureBox1.Image = ScreenCaptureForDisplay;

                int extraHeight = captureButton.Height + 85;
                int newWidth = Math.Max(pictureBox1.Width + 40, this.MinimumSize.Width);
                int newHeight = Math.Max(pictureBox1.Height + extraHeight, this.MinimumSize.Height);

                this.Size = new Size(newWidth, newHeight);
                this.Refresh();
            });


        }

        string[] Tokenize(string text)
        {
            // Remove nikud
            string normalized = Regex.Replace(text, @"[\u0590-\u05C7]", "");

            normalized = Regex.Replace(
                normalized,
                @"\b([ל])[\s-]*([א-ת])",
                "$1- $2"
            );
            // Split into words
            string[] tokens = Regex.Split(normalized, @"\s+")
                                   .Where(t => t.Length > 0)
                                   .ToArray();

            if (targetWords.All(word => text.Contains(word)))
            {
                int start = -1;
                int end = -1;
                for (int i = 0; i < tokens.Length; i++)
                {
                    if (targetWords.Contains(tokens[i]))
                    {
                        if (start == -1) start = i;
                        end = i; // keep updating to last occurrence
                    }
                }

                if (start != -1 && end != -1)
                {

                    int length = end - start + 2;
                    if (length > tokens.Length)
                        tokens.Skip(start).Take(tokens.Length - start).ToArray();

                    return tokens.Skip(start).Take(length).ToArray();
                }
            }

            return tokens;
        }

        public Bitmap ScaleBitmap(Bitmap source, float scale)
        {
            int newWidth = (int)(source.Width * scale);
            int newHeight = (int)(source.Height * scale);

            var bmp = new Bitmap(newWidth, newHeight);
            using (var g = Graphics.FromImage(bmp))
            {
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                g.DrawImage(source, 0, 0, newWidth, newHeight);
            }

            return bmp;
        }

        private float LevinshtienDistance(string originalQuestion, string questionFromDb)
        {
            var sourceLength1 = originalQuestion.Length;
            var sourceLength2 = questionFromDb.Length;

            var matrix = new int[sourceLength1 + 1, sourceLength2 + 1];

            if (sourceLength1 == 0)
                return sourceLength2;

            if (sourceLength2 == 0)
                return sourceLength1;

            int i, j;
            for (i = 0; i <= sourceLength1; matrix[i, 0] = i++) { }
            for (j = 0; j <= sourceLength2; matrix[0, j] = j++) { }

            for (i = 1; i <= sourceLength1; i++)
            {
                for (j = 1; j <= sourceLength2; j++)
                {
                    var cost = (questionFromDb[j - 1] == originalQuestion[i - 1]) ? 0 : 1;

                    matrix[i, j] = Math.Min(Math.Min(matrix[i - 1, j] + 1, matrix[i - 1, j - 1] + cost), matrix[i, j - 1] + 1);

                }
            }

            return (1 - (float)matrix[i - 1, j - 1] / Math.Max(sourceLength2, sourceLength1));
        }
    }
}



public class MyItem
{
    public string question { get; set; }
    public string answer { get; set; }
}


