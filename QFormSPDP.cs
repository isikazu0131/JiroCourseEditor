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
    public partial class QFormSPDP : Form {
        public QFormSPDP() {
            InitializeComponent();
        }

        public bool isSP = true;

        private void QFormSPDP_Load(object sender, EventArgs e) {
            // サイズ変更不可
            this.MaximumSize = this.Size;
            this.MinimumSize = this.Size;
        }

        private void BtSP_Click(object sender, EventArgs e) {
            this.DialogResult = DialogResult.OK;
            isSP = true;
            this.Close();
        }

        private void BtDP_Click(object sender, EventArgs e) {
            this.DialogResult = DialogResult.OK;
            isSP = false;
            this.Close();
        }
    }
}
