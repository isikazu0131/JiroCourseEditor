using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace JiroPackEditor {
    /// <summary>
    /// 次回以降表示しない系メッセージボックス
    /// </summary>
    public partial class MessageBoxOnce : Form {
        /// <summary>
        /// 次回以降表示するかどうかのチェック
        /// true: 表示する / false: 表示しない
        /// </summary>
        public bool FlgNextView = true;

        public string msg = "";

        public MessageBoxOnce() {
            InitializeComponent();
        }

        private void MessageBoxOnce_Load(object sender, EventArgs e) {
            this.MaximumSize = this.Size;
            this.MinimumSize = this.Size;
            LbMsg.Text = msg;
        }

        private void BtOk_Click(object sender, EventArgs e) {
            FlgNextView = CbNextView.Checked;
            this.DialogResult = DialogResult.OK;
        }

        private void MessageBoxOnce_FormClosing(object sender, FormClosingEventArgs e) {
            FlgNextView = CbNextView.Checked;
            this.DialogResult = DialogResult.OK;
        }
    }
}
