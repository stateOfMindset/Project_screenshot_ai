using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using Tesseract;

namespace Project_screenshot_ai
{
    public partial class Form1 : Form
    {
        private Timer screenshotTimer;
        private bool isCapturing = false;
        private Rectangle selectedRegion = Rectangle.Empty;
        private OverlayForm overlay;
        private Bitmap ScreenCaptureForDisplay;

        public Form1()
        {
            InitializeComponent();
            screenshotTimer = new Timer();
            screenshotTimer.Interval = 1000;
            screenshotTimer.Tick += ScreenshotTimer_Tick;
            screenshotTimer.Tick += Capture_screenshot_Click;
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

        public static string SearchInJson(string extractedText, string jsonPath)
        {
            var json = File.ReadAllText(jsonPath);
            var items = JsonConvert.DeserializeObject<List<MyItem>>(json);
            var match = items.FirstOrDefault(i => i.question.Contains(extractedText));
            return match != null ? match.answer : "No match found.";
        }

        public void ShowResult(string result)
        {
            this.Invoke((MethodInvoker)delegate
            {
                resultLabel.Text = result; // resultLabel is a Label on your form
            });
        }

        private void ProcessScreenshot()
        {
            string tessDataPath = Path.Combine(Application.StartupPath, "Data", "tessdata");
            string jsonPath = Path.Combine(Application.StartupPath, "Data", "json", "stage_1.json");
            Bitmap screenshot;

            if (selectedRegion != RectangleF.Empty)
                screenshot = CaptureRegion(selectedRegion);
            else
                screenshot = CaptureScreen();

            Bitmap processed = Preprocess(screenshot);

            string text = ExtractHebrewText(processed, tessDataPath);
            char[] sp = { '\n', '\r' };

            string clean = Regex.Replace(text, "[\\[\\]<>]", "");

            string[] split = clean.Split(sp);
            List<string> lines = new List<string>();
            foreach (string s in split)
            {
                if (!string.IsNullOrWhiteSpace(s))
                {
                    Console.WriteLine("matches found - " + s);
                    lines.Add(s);
                }
            }

            string returnRes = string.Join(",", lines);
            Console.WriteLine($"returnRes = {returnRes}");
            string answer = SearchInJson(returnRes, jsonPath);

            string logDir = Path.Combine(Application.StartupPath, "Logs");
            Directory.CreateDirectory(logDir); 
            string logPath = Path.Combine(logDir, "debug_output.txt");
            string logEntry = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} | {text}{Environment.NewLine}";
            File.AppendAllText(logPath, logEntry, Encoding.UTF8);


            ShowResult(answer);

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
                for(int x = 0; x < gray.Width; x++)
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
            int averageBrightness = (int) (totalBrightness / totalPixels);

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

            ScreenCaptureForDisplay = CaptureRegion(selectedRegion);

            pictureBox1.SizeMode = PictureBoxSizeMode.AutoSize;
            pictureBox1.Image = ScreenCaptureForDisplay;

            int extraHeight = captureButton.Height + 85;
            int newWidth = Math.Max(pictureBox1.Width + 40 , this.MinimumSize.Width);
            int newHeight = Math.Max(pictureBox1.Height + extraHeight, this.MinimumSize.Height);

            this.Size = new Size(newWidth, newHeight);


        }


    }
}

    

        public class MyItem
        {
            public string question { get; set; }
            public string answer { get; set; }
        }
   

