using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Text.Json;
using System.Windows.Forms;
using System.IO;
using System.IO.Compression;

namespace JiroPackEditor {

    /// <summary>
    /// コースパッククラス
    /// </summary>
    public class TJP {

        /// <summary>
        /// パック名
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// パックフォルダの表示カラー（文字）
        /// </summary>
        public string PackFolderForeColor { get; set; }

        /// <summary>
        /// パックフォルダの表示カラー（背景）
        /// </summary>
        public string PackFolderBackColor { get; set; }

        /// <summary>
        /// 段位フォルダの表示カラー（文字）
        /// </summary>
        public string CourseFolderForeColor { get; set; }

        /// <summary>
        /// 段位フォルダの表示カラー（背景）
        /// </summary>
        public string CourseFolderBackColor { get; set; }

        /// <summary>
        /// 課題曲フォルダの表示カラー（文字）
        /// </summary>
        public string SongFolderForeColor { get; set; }

        /// <summary>
        /// 課題曲フォルダの表示カラー（背景）
        /// </summary>
        public string SongFolderBackColor { get; set; }

        /// <summary>
        /// TJCリスト
        /// </summary>
        public List<TJC> TJCs { get; set; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public TJP() {
            TJCs = new List<TJC>();
        }

        /// <summary>
        /// ファイルを読み込む
        /// </summary>
        /// <param name="inputPath"></param>
        /// <returns></returns>
        public static TJP Read(string inputPath) {
            try {
                if (!File.Exists(inputPath)) {
                    MessageBox.Show("ファイルが存在しません。");
                    return null;
                }
                string jsonStr = File.ReadAllText(inputPath);
                TJP coursePack = JsonSerializer.Deserialize<TJP>(jsonStr);
                if (coursePack == null) {
                    MessageBox.Show("ファイルの読み込みに失敗しました。ファイルが壊れている可能性があります。");
                    return null;
                }
                return coursePack;
            }
            catch (Exception ex) {
                MessageBox.Show($"ファイルの読み込みに失敗しました。:\r\n" +
                 $"{ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// ファイル書き込み
        /// </summary>
        /// <param name="coursePack"></param>
        public static bool Write(TJP coursePack, string outputPath) {
            try {
                string JsonStr = JsonSerializer.Serialize(coursePack);
                File.WriteAllText(outputPath, JsonStr);
                MessageBox.Show("パックファイルの保存に成功しました。");
                return true;
            }
            catch (Exception ex) {
                MessageBox.Show($"ファイルの保存に失敗しました。:\r\n" +
                                $"{ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// TJPが正しいかどうかチェックします
        /// </summary>
        /// <returns>エラーメッセージ</returns>
        public string Check() {
            if (String.IsNullOrEmpty(Name.Trim())) {
                return "パック名が入力されていないか、空です";
            }
            if (TJCs.Count == 0) {
                return "コースがありません";
            }
            if (TJCs.Any(x => String.IsNullOrEmpty(x.Name.Trim()))) {
                return "コース名が入力されていないコースがあります";
            }
            if (TJCs.Any(x => x.TJAs.Count(y => y != null) == 0)) {
                return "TJAが登録されていないコースがあります";
            }
            if (TJCs.Any(x => GrobalMethod.CheckName(x.TJDRed.Name) != "") || TJCs.Any(x => GrobalMethod.CheckName(x.TJDGold.Name) != "")) {
                return "赤合格名称または金合格名称に使用できない文字列が含まれています\r\n" +
                       "（使用できない文字：'\\', '/', ':', '*', '?', '\"', ' < ', ' > ', ' | '）";
            }
            if(this.Name.Contains("//") || TJCs.Any(x => x.Name.Contains("//"))) {
                return "パック名またはコース名に使用できない文字列が含まれています\r\n" +
                       "（使用できない文字列：\"//\"）";
            }
            return "";
        }

        /// <summary>
        /// エクスポート（圧縮機能付き）
        /// </summary>
        /// <param name="tjp"></param>
        /// <returns></returns>
        public static bool Export(TJP tjp, Setting setting, bool isZip) {
            var tjpErrorMessage = tjp.Check();
            if (tjpErrorMessage != "") {
                MessageBox.Show(tjpErrorMessage, "ざんねん", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            if (String.IsNullOrEmpty(setting.PackOutputFolderName)) {
                MessageBox.Show("出力先フォルダが設定されていません。\r\n" +
                                "上部メニュー＞「設定」＞「パックを出力するフォルダの設定」より設定を行ってください。", "ざんねん", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            // tjpを出力するフォルダ(完全パス)
            var OutputTJPDPath = Path.Combine(setting.PackOutputFolderName, GrobalMethod.CutInvalidChar(tjp.Name));
            DirectoryInfo OutputTJPDInfo = new DirectoryInfo(OutputTJPDPath);

            // すでにエクスポートされたパックが存在していた場合
            if (OutputTJPDInfo.Exists) {
                if (MessageBox.Show($"すでにエクスポート済みのパックが存在します。上書きしてエクスポートしますか？\r\n" +
                                   $"（いったん削除してからエクスポートします）", "確認", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.No) {
                    return false;
                }
                // 削除する
                try {
                    OutputTJPDInfo.Delete(true);
                }
                catch (DirectoryNotFoundException) {
                    MessageBox.Show("パックフォルダが見つかりませんでした。\r\n" +
                                    "ははーん、さてはさっきのウィンドウが表示されてる間にこっそり消しましたね？？？",
                                    "ざんねん",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Error);
                    return false;
                }
                catch (IOException) {
                    MessageBox.Show("パックフォルダが現在使用中です。\r\n" +
                                    "パックフォルダ内のファイルを開いているエディタ等を閉じてから再度お試しください。",
                                    "ざんねん",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Error);
                    return false;
                }

            }

            // 再作成する
            OutputTJPDInfo.Create();

            // 「段位」フォルダ、「課題曲」フォルダ、「tjd」フォルダを作る
            var courseDInfo = OutputTJPDInfo.CreateSubdirectory(Constants.DirectoryName.Course);
            var songDInfo = OutputTJPDInfo.CreateSubdirectory(Constants.DirectoryName.Songs);
            var tjdDInfo = OutputTJPDInfo.CreateSubdirectory(Constants.DirectoryName.tjd);


            // パック、段位、課題曲フォルダに対してGenre.iniを出力する
            if (GenreIni.Write(OutputTJPDInfo.FullName, tjp.Name, tjp.PackFolderBackColor, tjp.PackFolderForeColor) == false ||
                GenreIni.Write(courseDInfo.FullName, "段位", tjp.CourseFolderBackColor, tjp.CourseFolderForeColor) == false ||
                GenreIni.Write(songDInfo.FullName, "課題曲", tjp.SongFolderBackColor, tjp.SongFolderForeColor) == false) {
                MessageBox.Show("Genre.iniの作成に失敗しました。",
                                "ざんねん",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
                return false;
            }

            // TJC1つずつに対して処理を行う
            foreach (var tjc in tjp.TJCs) {
                tjc.Export(tjp.Name, OutputTJPDInfo);
            }

            // 圧縮処理を行う
            if (isZip) {
                CompressTJP(OutputTJPDPath);
            }

            return true;
        }

        /// <summary>
        /// みおすな段位風のTJPテンプレートを作ります
        /// 固定15コース
        /// </summary>
        /// <returns></returns>
        public static TJP CreateMJEtemp(bool isSP) {
            try {
                TJP MJEtjp = new TJP {
                    // SPかDPかで名前変更
                    Name = isSP ? "第n期みおすな式段位認定　～ Miosuna's Jiro Exam ○th Edition" : "第n期みおすな式DP段位認定　～ Miosuna's DP Exam ○th Edition",
                    PackFolderBackColor = ColorInfo.GetColorCode(Color.Black),
                    PackFolderForeColor = ColorInfo.GetColorCode(Color.White),
                    CourseFolderBackColor = ColorInfo.GetColorCode(Color.Black),
                    CourseFolderForeColor = ColorInfo.GetColorCode(Color.Red),
                    SongFolderBackColor = ColorInfo.GetColorCode(Color.Black),
                    SongFolderForeColor = ColorInfo.GetColorCode(Color.DeepSkyBlue),
                    TJCs = new List<TJC>()
                };

                foreach (var i in Enumerable.Range(1, 17)) {
                    TJC TJCforMDE = TJC.CreateForTJCMJE(isSP, i);
                    // TJDはDPの場合無効にする
                    TJCforMDE.isTJDEnabled = isSP;
                    MJEtjp.TJCs.Add(TJCforMDE);
                }

                return MJEtjp;
            }
            catch (Exception ex) {
                MessageBox.Show($"テンプレートの作成に失敗しました。:{ex}");
                return null;
            }
        }

        private static bool CompressTJP(string tjpDFullname) {
            try {
                string tjpZipFullname = tjpDFullname + ".zip";
                ZipFile.CreateFromDirectory(tjpDFullname, tjpZipFullname, CompressionLevel.Fastest, false, Encoding.GetEncoding("Shift_jis"));
                return true;
            }
            catch(Exception e){
                MessageBox.Show($"パックの圧縮に失敗しました。\r\n" +
                                $"{e.Message}",
                                "ざんねん");
                return false;
            }
        }
    }
}
