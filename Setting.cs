using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace JiroPackEditor {
    public class Setting {

        /// <summary>
        /// パックを出力するフォルダ
        /// </summary>
        public string PackOutputFolderName { get; set; }

        /// <summary>
        /// テストプレイのTJCを削除するか
        /// </summary>
        public bool IsTestTJCDelete { get; set; }
        
        /// <summary>
        /// TJD有効時にTJDの中身が両方「なし」だった場合のメッセージを表示するか
        /// true: 表示する / false: 表示しない
        /// </summary>
        public bool IsViewTJDNoneMsg { get; set; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public Setting() {
            IsTestTJCDelete = true;
        }
        
        /// <summary>
        /// 設定ファイル(json)を読み込みます
        /// </summary>
        /// <returns></returns>
        public static Setting Read() {
            Setting setting = new Setting();
            // 設定フォルダがなければ、一旦フォルダだけ作って設定クラスのインスタンスを生成し返す
            if (!Directory.Exists(Constants.FileName.SettingFolder)) {
                Directory.CreateDirectory(Constants.FileName.SettingFolder);
                return setting;
            }
            if (File.Exists(Constants.FileName.SettingMainFile)) {
                string jsonstr = File.ReadAllText(Constants.FileName.SettingMainFile);
                setting = JsonSerializer.Deserialize<Setting>(jsonstr);
            }
            return setting;
        }

        /// <summary>
        /// 設定ファイル(json)を書き込みます
        /// </summary>
        /// <param name="setting"></param>
        public static void Write(Setting setting) {
            try {
                string jsonstr = JsonSerializer.Serialize(setting);
                File.WriteAllText(Constants.FileName.SettingMainFile, jsonstr);
            }
            catch (Exception ex) {
                MessageBox.Show($"設定ファイルの保存に失敗しました。:\r\n" +
                                $"{ex.Message}");
            }
        }
    }
}
