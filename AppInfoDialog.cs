using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace JiroPackEditor {
    public partial class AppInfoDialog : Form {
        public AppInfoDialog() {
            InitializeComponent();
        }

        private void AppInfoDialog_Load(object sender, EventArgs e) {
            this.MaximumSize = this.Size;
            this.MinimumSize = this.Size;

            LbInfo.Text = $"{Constants.AppInfo.Name}\r\n" +
                          $"バージョン : {Constants.AppInfo.Version}\r\n" +
                          $"開発者 : いしかず(Discord : @iskz / @14kz)\r\n" +
                          $"今この画面で流してえな～～～って思った曲 : シャイニングスター\r\n" +
                          $"結構スペース余っちゃったね : そうだね";
            LinkGitHub.Links.Add(0, LinkGitHub.Text.Length, Constants.AppInfo.GitHubLink);
        }

        private void LinkGitHub_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {
            try {
                Process.Start(new ProcessStartInfo(Constants.AppInfo.GitHubLink) { UseShellExecute = true });
            }
            catch (Exception ex) {
                MessageBox.Show("リンクを開けませんでした。");
            }
        }

        private void pictureBox1_Click(object sender, EventArgs e) {
            MessageBox.Show("かわいいでしょ");
        }

        private void button1_Click(object sender, EventArgs e) {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}
