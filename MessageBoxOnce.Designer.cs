namespace JiroPackEditor {
    partial class MessageBoxOnce {
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
            this.LbMsg = new System.Windows.Forms.Label();
            this.BtOk = new System.Windows.Forms.Button();
            this.CbNextView = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // LbMsg
            // 
            this.LbMsg.Location = new System.Drawing.Point(12, 9);
            this.LbMsg.Name = "LbMsg";
            this.LbMsg.Size = new System.Drawing.Size(219, 45);
            this.LbMsg.TabIndex = 0;
            this.LbMsg.Text = "label1";
            // 
            // BtOk
            // 
            this.BtOk.Location = new System.Drawing.Point(145, 53);
            this.BtOk.Name = "BtOk";
            this.BtOk.Size = new System.Drawing.Size(75, 23);
            this.BtOk.TabIndex = 1;
            this.BtOk.Text = "OK";
            this.BtOk.UseVisualStyleBackColor = true;
            this.BtOk.Click += new System.EventHandler(this.BtOk_Click);
            // 
            // CbNextView
            // 
            this.CbNextView.AutoSize = true;
            this.CbNextView.Location = new System.Drawing.Point(14, 57);
            this.CbNextView.Name = "CbNextView";
            this.CbNextView.Size = new System.Drawing.Size(125, 16);
            this.CbNextView.TabIndex = 2;
            this.CbNextView.Text = "次回以降表示しない";
            this.CbNextView.UseVisualStyleBackColor = true;
            // 
            // MessageBoxOnce
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(233, 87);
            this.Controls.Add(this.CbNextView);
            this.Controls.Add(this.BtOk);
            this.Controls.Add(this.LbMsg);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "MessageBoxOnce";
            this.Text = "MessageBoxOnce";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MessageBoxOnce_FormClosing);
            this.Load += new System.EventHandler(this.MessageBoxOnce_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label LbMsg;
        private System.Windows.Forms.Button BtOk;
        private System.Windows.Forms.CheckBox CbNextView;
    }
}