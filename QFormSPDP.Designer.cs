namespace JiroCourseEditor {
    partial class QFormSPDP {
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
            this.label1 = new System.Windows.Forms.Label();
            this.BtSP = new System.Windows.Forms.Button();
            this.BtDP = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(15, 10);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(168, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "どちらで作成するかを選んでください";
            // 
            // BtSP
            // 
            this.BtSP.Location = new System.Drawing.Point(74, 47);
            this.BtSP.Name = "BtSP";
            this.BtSP.Size = new System.Drawing.Size(75, 23);
            this.BtSP.TabIndex = 1;
            this.BtSP.Text = "SP";
            this.BtSP.UseVisualStyleBackColor = true;
            this.BtSP.Click += new System.EventHandler(this.BtSP_Click);
            // 
            // BtDP
            // 
            this.BtDP.Location = new System.Drawing.Point(74, 76);
            this.BtDP.Name = "BtDP";
            this.BtDP.Size = new System.Drawing.Size(75, 23);
            this.BtDP.TabIndex = 2;
            this.BtDP.Text = "DP";
            this.BtDP.UseVisualStyleBackColor = true;
            this.BtDP.Click += new System.EventHandler(this.BtDP_Click);
            // 
            // QFormSPDP
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(227, 131);
            this.Controls.Add(this.BtDP);
            this.Controls.Add(this.BtSP);
            this.Controls.Add(this.label1);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "QFormSPDP";
            this.Text = "QFormSPDP";
            this.Load += new System.EventHandler(this.QFormSPDP_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button BtSP;
        private System.Windows.Forms.Button BtDP;
    }
}