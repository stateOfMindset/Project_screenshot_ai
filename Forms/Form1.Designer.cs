namespace Project_screenshot_ai
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.resultLabel = new System.Windows.Forms.Label();
            this.captureButton = new System.Windows.Forms.Button();
            this.selectRegionButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // resultLabel
            // 
            this.resultLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 36F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(177)));
            this.resultLabel.Location = new System.Drawing.Point(173, 9);
            this.resultLabel.Name = "resultLabel";
            this.resultLabel.Size = new System.Drawing.Size(432, 258);
            this.resultLabel.TabIndex = 0;
            this.resultLabel.Text = "No Data Yet";
            this.resultLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // captureButton
            // 
            this.captureButton.Location = new System.Drawing.Point(284, 339);
            this.captureButton.Name = "captureButton";
            this.captureButton.Size = new System.Drawing.Size(201, 53);
            this.captureButton.TabIndex = 1;
            this.captureButton.Text = "Start capturing";
            this.captureButton.UseVisualStyleBackColor = true;
            this.captureButton.Click += new System.EventHandler(this.captureButton_Click);
            // 
            // selectRegionButton
            // 
            this.selectRegionButton.Location = new System.Drawing.Point(570, 343);
            this.selectRegionButton.Name = "selectRegionButton";
            this.selectRegionButton.Size = new System.Drawing.Size(179, 48);
            this.selectRegionButton.TabIndex = 2;
            this.selectRegionButton.Text = "select Region Button";
            this.selectRegionButton.UseVisualStyleBackColor = true;
            this.selectRegionButton.Click += new System.EventHandler(this.selectRegionButton_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.selectRegionButton);
            this.Controls.Add(this.captureButton);
            this.Controls.Add(this.resultLabel);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label resultLabel;
        private System.Windows.Forms.Button captureButton;
        private System.Windows.Forms.Button selectRegionButton;
    }
}

