namespace JiroCourseEditor {
    partial class AppInfoDialog {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AppInfoDialog));
            this.LbInfo = new System.Windows.Forms.Label();
            this.LinkGitHub = new System.Windows.Forms.LinkLabel();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.label1 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // LbInfo
            // 
            this.LbInfo.AutoSize = true;
            this.LbInfo.Font = new System.Drawing.Font("ＭＳ ゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.LbInfo.Location = new System.Drawing.Point(12, 9);
            this.LbInfo.Name = "LbInfo";
            this.LbInfo.Size = new System.Drawing.Size(29, 12);
            this.LbInfo.TabIndex = 0;
            this.LbInfo.Text = "Info";
            // 
            // LinkGitHub
            // 
            this.LinkGitHub.AutoSize = true;
            this.LinkGitHub.Location = new System.Drawing.Point(12, 223);
            this.LinkGitHub.Name = "LinkGitHub";
            this.LinkGitHub.Size = new System.Drawing.Size(234, 12);
            this.LinkGitHub.TabIndex = 1;
            this.LinkGitHub.TabStop = true;
            this.LinkGitHub.Text = "GitHubへ進む(当アプリのソースコードが見れます)";
            this.LinkGitHub.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.LinkGitHub_LinkClicked);
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pictureBox1.Image = global::JiroCourseEditor.Properties.Resources.miosunaicon;
            this.pictureBox1.Location = new System.Drawing.Point(357, 122);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(85, 84);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 2;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.Click += new System.EventHandler(this.pictureBox1_Click);
            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("ＭＳ ゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label1.Location = new System.Drawing.Point(341, 85);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(121, 34);
            this.label1.TabIndex = 3;
            this.label1.Text = "　私が作りました\r\n＼　フハハハハ　／";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(357, 212);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(85, 23);
            this.button1.TabIndex = 4;
            this.button1.Text = "OK";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // AppInfoDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(454, 247);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.LinkGitHub);
            this.Controls.Add(this.LbInfo);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AppInfoDialog";
            this.Text = "このアプリについて";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.AppInfoDialog_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label LbInfo;
        private System.Windows.Forms.LinkLabel LinkGitHub;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button button1;
    }
}