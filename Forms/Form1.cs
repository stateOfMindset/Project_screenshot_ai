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

        public Form1()
        {
            InitializeComponent();
            screenshotTimer = new Timer();
            screenshotTimer.Interval = 1000;
            screenshotTimer.Tick += ScreenshotTimer_Tick;
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

            string text = ExtractHebrewText(screenshot, tessDataPath);
            char[] sp = { '\n', '\r' };
            string[] split = text.Split(sp);
            string returnRes = "";
            foreach (string s in split)
            {
                if (!string.IsNullOrWhiteSpace(s))
                {
                    Console.WriteLine("matches found - " + s);
                    returnRes = String.Join("," , s);
                }
            }
            Console.WriteLine($"returnRes = {returnRes}");
            string answer = SearchInJson(text, jsonPath);
            ShowResult(answer);

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

       
    }
}

    

        public class MyItem
        {
            public string question { get; set; }
            public string answer { get; set; }
        }
   

