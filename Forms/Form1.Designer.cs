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
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.Capture_screenshot = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // resultLabel
            // 
            this.resultLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.resultLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(177)));
            this.resultLabel.Location = new System.Drawing.Point(48, 418);
            this.resultLabel.Name = "resultLabel";
            this.resultLabel.Size = new System.Drawing.Size(115, 48);
            this.resultLabel.TabIndex = 0;
            this.resultLabel.Text = "No Data Yet";
            this.resultLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // captureButton
            // 
            this.captureButton.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.captureButton.Location = new System.Drawing.Point(195, 413);
            this.captureButton.Name = "captureButton";
            this.captureButton.Size = new System.Drawing.Size(201, 53);
            this.captureButton.TabIndex = 1;
            this.captureButton.Text = "Start capturing";
            this.captureButton.UseVisualStyleBackColor = true;
            this.captureButton.Click += new System.EventHandler(this.captureButton_Click);
            // 
            // selectRegionButton
            // 
            this.selectRegionButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.selectRegionButton.Location = new System.Drawing.Point(612, 421);
            this.selectRegionButton.Name = "selectRegionButton";
            this.selectRegionButton.Size = new System.Drawing.Size(179, 48);
            this.selectRegionButton.TabIndex = 2;
            this.selectRegionButton.Text = "select Region Button";
            this.selectRegionButton.UseVisualStyleBackColor = true;
            this.selectRegionButton.Click += new System.EventHandler(this.selectRegionButton_Click);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pictureBox1.Location = new System.Drawing.Point(51, 12);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(716, 388);
            this.pictureBox1.TabIndex = 3;
            this.pictureBox1.TabStop = false;
            // 
            // Capture_screenshot
            // 
            this.Capture_screenshot.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.Capture_screenshot.Location = new System.Drawing.Point(441, 422);
            this.Capture_screenshot.Name = "Capture_screenshot";
            this.Capture_screenshot.Size = new System.Drawing.Size(133, 46);
            this.Capture_screenshot.TabIndex = 4;
            this.Capture_screenshot.Text = "Capture_screenshot";
            this.Capture_screenshot.UseVisualStyleBackColor = true;
            this.Capture_screenshot.Click += new System.EventHandler(this.Capture_screenshot_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(820, 490);
            this.Controls.Add(this.Capture_screenshot);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.selectRegionButton);
            this.Controls.Add(this.captureButton);
            this.Controls.Add(this.resultLabel);
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label resultLabel;
        private System.Windows.Forms.Button captureButton;
        private System.Windows.Forms.Button selectRegionButton;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Button Capture_screenshot;
    }
}

