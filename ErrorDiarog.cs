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
    public partial class ErrorDialog : Form {

        private string ErrorStr = "";

        public ErrorDialog() {
            InitializeComponent();
        }

        private void ErrorDialog_Load(object sender, EventArgs e) {
            LbError.BackColor = Color.Transparent;
            LbError.Text = ErrorStr;
            System.Media.SystemSounds.Hand.Play();
        }

        private void BtOk_Click(object sender, EventArgs e) {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        public void ViewDialog(string errorStr) {
            ErrorStr = errorStr;
            this.ShowDialog();
        }
    }
}
