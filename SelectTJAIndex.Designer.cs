namespace JiroCourseEditor {
    partial class SelectTJAIndex {
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SelectTJAIndex));
            this.label1 = new System.Windows.Forms.Label();
            this.CbTJANum = new System.Windows.Forms.ComboBox();
            this.BtOK = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(260, 35);
            this.label1.TabIndex = 0;
            this.label1.Text = "ドラッグアンドドロップしたTJAファイルをコース内の何番目に入れるか選択してください";
            // 
            // CbTJANum
            // 
            this.CbTJANum.BackColor = System.Drawing.Color.White;
            this.CbTJANum.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.CbTJANum.FormattingEnabled = true;
            this.CbTJANum.Items.AddRange(new object[] {
            "1曲目",
            "2曲目",
            "3曲目",
            "4曲目",
            "5曲目"});
            this.CbTJANum.Location = new System.Drawing.Point(14, 47);
            this.CbTJANum.Name = "CbTJANum";
            this.CbTJANum.Size = new System.Drawing.Size(177, 20);
            this.CbTJANum.TabIndex = 1;
            // 
            // BtOK
            // 
            this.BtOK.Location = new System.Drawing.Point(197, 47);
            this.BtOK.Name = "BtOK";
            this.BtOK.Size = new System.Drawing.Size(75, 23);
            this.BtOK.TabIndex = 2;
            this.BtOK.Text = "OK";
            this.BtOK.UseVisualStyleBackColor = true;
            this.BtOK.Click += new System.EventHandler(this.BtOK_Click);
            // 
            // SelectTJAIndex
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 82);
            this.Controls.Add(this.BtOK);
            this.Controls.Add(this.CbTJANum);
            this.Controls.Add(this.label1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SelectTJAIndex";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.SelectTJAIndex_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox CbTJANum;
        private System.Windows.Forms.Button BtOK;
    }
}