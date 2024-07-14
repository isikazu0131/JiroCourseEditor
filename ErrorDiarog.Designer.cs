namespace JiroPackEditor {
    partial class ErrorDialog {
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ErrorDialog));
            this.LbError = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.BtOk = new System.Windows.Forms.Button();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            this.SuspendLayout();
            // 
            // LbError
            // 
            this.LbError.BackColor = System.Drawing.Color.Transparent;
            this.LbError.Font = new System.Drawing.Font("MS UI Gothic", 25F);
            this.LbError.Location = new System.Drawing.Point(12, 9);
            this.LbError.Name = "LbError";
            this.LbError.Size = new System.Drawing.Size(497, 314);
            this.LbError.TabIndex = 0;
            this.LbError.Text = "label1";
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::JiroPackEditor.Properties.Resources.ng;
            this.pictureBox1.Location = new System.Drawing.Point(12, 221);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(606, 345);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 1;
            this.pictureBox1.TabStop = false;
            // 
            // BtOk
            // 
            this.BtOk.Font = new System.Drawing.Font("MS UI Gothic", 108F);
            this.BtOk.Location = new System.Drawing.Point(471, 326);
            this.BtOk.Name = "BtOk";
            this.BtOk.Size = new System.Drawing.Size(412, 250);
            this.BtOk.TabIndex = 2;
            this.BtOk.Text = "オッケ";
            this.BtOk.UseVisualStyleBackColor = true;
            this.BtOk.Click += new System.EventHandler(this.BtOk_Click);
            // 
            // pictureBox2
            // 
            this.pictureBox2.Image = global::JiroPackEditor.Properties.Resources.ng;
            this.pictureBox2.Location = new System.Drawing.Point(436, -7);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(483, 299);
            this.pictureBox2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox2.TabIndex = 3;
            this.pictureBox2.TabStop = false;
            // 
            // ErrorDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(813, 562);
            this.Controls.Add(this.BtOk);
            this.Controls.Add(this.LbError);
            this.Controls.Add(this.pictureBox2);
            this.Controls.Add(this.pictureBox1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "ErrorDialog";
            this.Text = "ダメ！！！！！！！！！！！！！！";
            this.Load += new System.EventHandler(this.ErrorDialog_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label LbError;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Button BtOk;
        private System.Windows.Forms.PictureBox pictureBox2;
    }
}