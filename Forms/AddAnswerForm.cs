using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace Project_screenshot_ai.Forms
{
    public partial class AddAnswerForm : Form
    {
        public int SelectedNumber { get; private set; }
        public string Answer { get; private set; }
        public Point location { get; private set; }
        public string question { get; private set; }


        public AddAnswerForm(string question , Point location)
        {
            InitializeComponent();

            this.location = location;
            this.question = question;

            for (int i = 1; i <= 6; i++)
                comboBoxNumber.Items.Add(i);

            setSettings();
            this.Load += AddAnswerForm_Load;
           

        }
  
        private void AddAnswerForm_Load(object sender, EventArgs e)
        {       
               this.Location = new Point( location.X, location.Y);
               this.Refresh();
        }


        private void setSettings()
        {
            comboBoxNumber.SelectedIndex = 0;
            if(question != null)
                lblQuestion.AppendText(question);



            lblQuestion.RightToLeft = RightToLeft.Yes;
            lblQuestion.ImeMode = ImeMode.On;
            InputLanguage.CurrentInputLanguage = InputLanguage.FromCulture(new System.Globalization.CultureInfo("he-IL"));

            this.StartPosition = FormStartPosition.Manual;
            this.FormBorderStyle = FormBorderStyle.None;
            this.TopMost = true;
            this.Opacity = 0.85;
            this.BackColor = Color.Purple;
            this.ShowInTaskbar = false;
            this.BringToFront();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            // Get values
            SelectedNumber = (int)comboBoxNumber.SelectedItem;
            Answer = txtAnswer.Text.Trim();
            string question_fromInput = lblQuestion.Text;

            if (string.IsNullOrEmpty(Answer) || string.IsNullOrEmpty(question_fromInput))
            {
                MessageBox.Show("Please enter an answer!", "Missing Data",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DialogResult = DialogResult.OK;
            
            SaveToJson(SelectedNumber , question_fromInput, Answer);
            Close();
        }

        public void SaveToJson(int stageNumber, string question, string answer)
        {
            string fileName = $"stage_{stageNumber}.json";
            string filePath = (Path.Combine(GlobalData.RootJsonPath, fileName));

            List<QA> entries = new List<QA>();

            // If file already exists, load it first
            if (File.Exists(filePath))
            {
                string json = File.ReadAllText(filePath);
                entries = JsonConvert.DeserializeObject<List<QA>>(json) ?? new List<QA>();
            }

            // Avoid duplicates
            bool exists = entries.Exists(e => e.Question == question);
            if (!exists)
            {
                entries.Add(new QA { Question = question, Answer = answer });
            }

            // Save back
            string newJson = JsonConvert.SerializeObject(entries, Formatting.Indented);
            File.WriteAllText(filePath, newJson);
        }

    }

}
