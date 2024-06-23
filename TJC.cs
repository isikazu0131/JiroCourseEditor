using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace JiroCourseEditor {
    /// <summary>
    /// TJCを扱うクラス
    /// </summary>
    public class TJC {

        /// <summary>
        /// コース名
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// ライフ値
        /// </summary>
        public int Life { get; set; }

        /// <summary>
        /// ナンバリング
        /// </summary>
        public int Number { get; set; }

        /// <summary>
        /// TJAリスト
        /// </summary>
        public List<TJA> TJAs { get; set; }

        /// <summary>
        /// TJCのナンバリングをするか
        /// Default: True
        /// </summary>
        public bool IsNumberingEnable { get; set; }

        /// <summary>
        /// 各楽曲のTITLEを非表示にするか
        /// Default: True
        /// </summary>
        public bool IsTitleHide { get; set; }

        /// <summary>
        /// 課題曲フォルダ内各レベルフォルダ表示カラー（背景）
        /// </summary>
        public string LevelBackColor { get; set; }

        /// <summary>
        /// 課題曲フォルダ内各レベルフォルダ表示カラー（文字）
        /// </summary>
        public string LevelForeColor { get; set; }

        /// <summary>
        /// TJDを有効にするか
        /// Default: True
        /// </summary>
        public bool isTJDEnabled { get; set; }

        /// <summary>
        /// 赤合格条件TJD
        /// </summary>
        public TJD TJDRed { get; set; }

        /// <summary>
        /// 金合格条件TJD
        /// </summary>
        public TJD TJDGold { get; set; }

        /// <summary>
        /// TJDを１つのファイルに統合するか
        /// </summary>
        public bool isTJDCombine { get; set; }

        public TJC() {
            // 5曲MAX
            // 上書きしたり削除したりしまくるので、
            // TJAsそのものをnullにするよりはこうしたほうが管理が楽
            TJAs = new List<TJA>() {
                null,
                null,
                null,
                null,
                null,
            };
            TJDRed = new TJD("赤合格");
            TJDGold = new TJD("金合格");
            LevelBackColor = ColorInfo.GetColorCode(Color.White);
            LevelForeColor = ColorInfo.GetColorCode(Color.Black);
            IsNumberingEnable = true;
            IsTitleHide = true;
            isTJDEnabled = true;
        }

        /// <summary>
        /// コース内の総ノーツ数を計算します
        /// </summary>
        /// <returns></returns>
        public int TotalNoteCount() {
            var notNullTJAs = TJAs.Where(tja => tja != null).ToList();
            if (notNullTJAs.Any()) return notNullTJAs.Sum(x => x.NotesCount);
            else return 0;
        }

        /// <summary>
        /// コース内の総再生時間を計算します
        /// </summary>
        /// <returns></returns>
        public double TotalOggTime() {
            var notNullTJAs = TJAs.Where(tja => tja != null).ToList();
            if (notNullTJAs.Any()) return notNullTJAs.Sum(x => x.MusicPlayTime);
            else return 0;
        }

        /// <summary>
        /// コース内平均難易度を計算します
        /// </summary>
        /// <returns></returns>
        public double AvgLevel() {
            var notNullTJAs = TJAs.Where(tja => tja != null).ToList();
            if (notNullTJAs.Any()) return notNullTJAs.Average(x => x.LEVEL);
            else return 0;
        }


        /// <summary>
        /// コース内のファイルサイズを合計する
        /// </summary>
        /// <returns></returns>
        public long GetTJAsSize() {
            var notNullTJAs = TJAs.Where(tja => tja != null).ToList();
            if (notNullTJAs.Any()) {
                long totalSize = 0;
                foreach (TJA tja in notNullTJAs) {
                    totalSize += GetDirectorySize(new DirectoryInfo(Path.GetDirectoryName(tja.FullName)));
                }
                return totalSize;
            } else return 0;
        }

        /// <summary>
        /// ディレクトリ内のファイルサイズを取得
        /// </summary>
        /// <param name="dInfo"></param>
        /// <returns></returns>
        private long GetDirectorySize(DirectoryInfo dInfo) {
            long totalSize = 0;
            if (dInfo != null) {
                var fInfos = dInfo.GetFiles();
                foreach (var f in fInfos) {
                    totalSize += f.Length;
                }
            }
            return totalSize;
        }

        /// <summary>
        /// MJE向けTJC
        /// </summary>
        /// <param name="Level"></param>
        /// <returns></returns>
        public static TJC CreateForTJCMJE(bool isSP, int Level) {
            TJC tjc = new TJC();
            tjc.Name = $"Level {Level}";
            // フォルダカラー変更
            switch (Level) {
                case 1:
                    tjc.LevelBackColor = ColorInfo.GetColorCode(Color.FromArgb(255, 153, 204));
                    tjc.LevelForeColor = ColorInfo.GetColorCode(Color.FromArgb(0, 0, 0));
                    break;
                case 2:
                    tjc.LevelBackColor = ColorInfo.GetColorCode(Color.FromArgb(255, 0, 0));
                    tjc.LevelForeColor = ColorInfo.GetColorCode(Color.FromArgb(0, 0, 0));
                    break;
                case 3:
                    tjc.LevelBackColor = ColorInfo.GetColorCode(Color.FromArgb(237, 125, 49));
                    tjc.LevelForeColor = ColorInfo.GetColorCode(Color.FromArgb(0, 0, 0));
                    break;
                case 4:
                    tjc.LevelBackColor = ColorInfo.GetColorCode(Color.FromArgb(255, 255, 0));
                    tjc.LevelForeColor = ColorInfo.GetColorCode(Color.FromArgb(0, 0, 0));
                    break;
                case 5:
                    tjc.LevelBackColor = ColorInfo.GetColorCode(Color.FromArgb(146, 208, 80));
                    tjc.LevelForeColor = ColorInfo.GetColorCode(Color.FromArgb(0, 0, 0));
                    break;
                case 6:
                    tjc.LevelBackColor = ColorInfo.GetColorCode(Color.FromArgb(84, 129, 53));
                    tjc.LevelForeColor = ColorInfo.GetColorCode(Color.FromArgb(0, 0, 0));
                    break;
                case 7:
                    tjc.LevelBackColor = ColorInfo.GetColorCode(Color.FromArgb(0, 176, 240));
                    tjc.LevelForeColor = ColorInfo.GetColorCode(Color.FromArgb(0, 0, 0));
                    break;
                case 8:
                    tjc.LevelBackColor = ColorInfo.GetColorCode(Color.FromArgb(0, 112, 192));
                    tjc.LevelForeColor = ColorInfo.GetColorCode(Color.FromArgb(0, 0, 0));
                    break;
                case 9:
                    tjc.LevelBackColor = ColorInfo.GetColorCode(Color.FromArgb(219, 105, 255));
                    tjc.LevelForeColor = ColorInfo.GetColorCode(Color.FromArgb(0, 0, 0));
                    break;
                case 10:
                    tjc.LevelBackColor = ColorInfo.GetColorCode(Color.FromArgb(196, 0, 196));
                    tjc.LevelForeColor = ColorInfo.GetColorCode(Color.FromArgb(0, 0, 0));
                    break;
                case 11:
                    tjc.LevelBackColor = ColorInfo.GetColorCode(Color.FromArgb(0, 0, 0));
                    tjc.LevelForeColor = ColorInfo.GetColorCode(Color.FromArgb(255, 255, 255));
                    break;
                case 12:
                    tjc.LevelBackColor = ColorInfo.GetColorCode(Color.FromArgb(0, 0, 0));
                    tjc.LevelForeColor = ColorInfo.GetColorCode(Color.FromArgb(255, 255, 0));
                    break;
                case 13:
                    tjc.LevelBackColor = ColorInfo.GetColorCode(Color.FromArgb(0, 0, 0));
                    tjc.LevelForeColor = ColorInfo.GetColorCode(Color.FromArgb(0, 255, 0));
                    break;
                case 14:
                    tjc.LevelBackColor = ColorInfo.GetColorCode(Color.FromArgb(0, 0, 0));
                    tjc.LevelForeColor = ColorInfo.GetColorCode(Color.FromArgb(0, 255, 255));
                    break;
                case 15:
                    tjc.LevelBackColor = ColorInfo.GetColorCode(Color.FromArgb(0, 0, 0));
                    tjc.LevelForeColor = ColorInfo.GetColorCode(Color.FromArgb(196, 0, 196));
                    break;
                case 16:
                    tjc.LevelBackColor = ColorInfo.GetColorCode(Color.FromArgb(0, 0, 0));
                    tjc.LevelForeColor = ColorInfo.GetColorCode(Color.FromArgb(192, 0, 0));
                    break;
                case 17:
                    tjc.LevelBackColor = ColorInfo.GetColorCode(Color.FromArgb(0, 0, 0));
                    tjc.LevelForeColor = ColorInfo.GetColorCode(Color.FromArgb(112, 48, 160));
                    break;
                default:
                    break;
            }
            // 条件設定
            tjc.TJDRed = TJD.CreateTJDforMJE(isSP, true, Level);
            tjc.TJDGold = TJD.CreateTJDforMJE(isSP, false, Level);
            return tjc;
        }

        /// <summary>
        /// テストプレイする
        /// </summary>
        /// <returns>テストプレイで生成したTJC</returns>
        public FileInfo TestPlay() {
            if (!this.TJAs.Any(x => x != null)) {
                MessageBox.Show("TJAを登録してください", "ざんねん", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }

            Process[] jiroProcesses = Process.GetProcessesByName("taikojiro");
            if (jiroProcesses.Length == 0) {
                MessageBox.Show("太鼓さん次郎を起動してください", "ざんねん", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
            Process jiroProcess = jiroProcesses[0];
            FileInfo JiroFInfo = new FileInfo(jiroProcess.MainModule.FileName);
            var JiroDInfo = JiroFInfo.Directory;

            // 対象のTJAのうち、1つでも起動している次郎のディレクトリ内にない場合
            if (this.TJAs.Where(x => x != null).Any(x => x.FullName.StartsWith(JiroDInfo.FullName) == false)) {
                MessageBox.Show("コースのTJAが、すべて起動している太鼓さん次郎のディレクトリ内に存在するようにしてください",
                                "ざんねん", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }

            // テスト用tjcの書き込み
            var tjcFInfo = WriteTestTJC(JiroFInfo);

            if (tjcFInfo == null) {
                MessageBox.Show("テスト用TJCの書き込みに失敗しました", "ざんねん", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }

            // 次郎にドラッグアンドドロップするTJC
            MmdDropFile DropTJC = new MmdDropFile(tjcFInfo.FullName);

            // ドロップ操作
            Task.Run(() => {
                FileDrop.DropFile(jiroProcess.MainWindowHandle, DropTJC);
            });

            return tjcFInfo;
        }

        /// <summary>
        /// テスト用TJCを出力します
        /// </summary>
        /// <param name="JiroFInfo"></param>
        /// <returns></returns>
        private FileInfo WriteTestTJC(FileInfo JiroFInfo) {
            try {
                DateTime dateTime = DateTime.Now;

                string tjcname = $@"{JiroFInfo.Directory.FullName}\{this.Name}_test_{dateTime.ToString("yyyyMMddHHmmss")}.tjc";

                using (StreamWriter sw = new StreamWriter(tjcname, false, Encoding.GetEncoding("Shift_jis"))) {
                    // tjcに書き込む内容
                    List<string> lines = new List<string>();
                    lines.Add($"TITLE:{this.Name}_テスト段位_{dateTime.ToString("yyyyMMddHHmmss")}\r\n" +
                              $"LIFE:{this.Life}\r\n");

                    foreach (var tja in this.TJAs) {
                        if (tja == null) continue;
                        // tjaの相対パスを取得して書いていく
                        string relativePath = tja.FullName.Replace(JiroFInfo.DirectoryName + @"\", "");
                        lines.Add($"SONG:{relativePath}");
                    }

                    // 1行ずつ書き込み
                    foreach (var line in lines) {
                        sw.WriteLine(line);
                    }
                }
                return new FileInfo(tjcname);
            }
            catch (Exception e) {
                return null;
            }
        }

        /// <summary>
        /// TJAのTITLEを？？？にします
        /// </summary>
        /// <param name="tjaFInfo"></param>
        /// <returns></returns>
        private bool TJATITLEHide(FileInfo tjaFInfo) {
            try {
                TJA tja = new TJA(tjaFInfo);
                var tjaLines = File.ReadAllText(tjaFInfo.FullName, Encoding.GetEncoding("Shift_jis"));

                tjaLines = tjaLines.Replace($"TITLE:{tja.TITLE}", "TITLE:？？？　　　　　　　　　　　　　　　　　　　　　　　　　　　　　　　　　　　　　　　　" + tja.TITLE);

                using (StreamWriter sw = new StreamWriter(tjaFInfo.FullName, false, Encoding.GetEncoding("Shift_jis"))) {
                    sw.WriteLine(tjaLines);
                }

                return true;
            }
            catch (Exception e) {
                return false;
            }
        }

        /// <summary>
        /// tjcの内容をエクスポートし、tjaを移動させます
        /// </summary>
        /// <param name="packDInfo">パックフォルダ</param>
        public bool Export(string tjpName, DirectoryInfo packDInfo) {
            try {
                
                // ナンバリングした数値
                string numberedTJCName = IsNumberingEnable ? $"{this.Number.ToString("000")} {this.Name}" : this.Name;
                // tjcファイルの完全パス
                // (エクスポート先)\(パック名)\段位\(コース名).tjc
                string tjcPath = Path.Combine(packDInfo.FullName,
                                              Constants.DirectoryName.Course,
                                              numberedTJCName + Constants.Extention.TJC);
                using (StreamWriter sw = new StreamWriter(tjcPath, false, Encoding.GetEncoding("Shift_jis"))) {
                    // tjcに書き込む内容
                    List<string> lines = new List<string>();
                    lines.Add($"TITLE:{this.Name}\r\n" +
                              $"LIFE:{this.Life}\r\n");

                    foreach (var tja in this.TJAs) {
                        if (tja == null) continue;
                        // tjaの相対パスを取得して書いていく
                        // (パック名)\課題曲\(コース名)\(tja名)
                        string relativePath = Path.Combine(tjpName,
                                                           Constants.DirectoryName.Songs,
                                                           numberedTJCName,
                                                           tja.FileName.Replace(Path.GetExtension(tja.FileName), ""),
                                                           tja.FileName);
                        lines.Add($"SONG:{relativePath}");
                    }

                    // 1行ずつ書き込み
                    foreach (var line in lines) {
                        sw.WriteLine(line);
                    }
                }

                // 課題曲＞コース名フォルダ作成
                // (エクスポート先)\(パック名)\課題曲\(コース名)
                string levelDirectory = Path.Combine(packDInfo.FullName,
                                                    Constants.DirectoryName.Songs,
                                                    numberedTJCName
                                                    );
                DirectoryInfo songDInfo = Directory.CreateDirectory(levelDirectory);

                // tjaを1つずつコピーする
                foreach (var tja in this.TJAs) {
                    if (tja == null) continue;
                    var oggInfo = new FileInfo(tja.FullName.Replace(tja.FileName, tja.WAVE));
                    var tjaInfo = new FileInfo(tja.FullName);
                    // tja一式出力先フォルダ
                    // (エクスポート先)\(パック名)\課題曲\(コース名)\(TITLE)
                    string outputpath = Path.Combine(levelDirectory,
                                                     Path.GetFileName(tja.FullName).Replace(Path.GetExtension(tja.FileName), "")
                                                     );
                    Directory.CreateDirectory(outputpath);

                    // 出力tja
                    string outputTJApath = Path.Combine(outputpath, tja.FileName);
                    // 出力ogg
                    string outputOGGpath = Path.Combine(outputpath, tja.WAVE);
                    // tja、oggをコピーする
                    var copiedTJA = tjaInfo.CopyTo(outputTJApath, true);
                    oggInfo.CopyTo(outputOGGpath, true);

                    // 楽曲名非表示
                    if (IsTitleHide) TJATITLEHide(copiedTJA);
                }

                // Genre.iniをエクスポート
                GenreIni.Write(songDInfo.FullName, this.Name, LevelBackColor, LevelForeColor);

                DirectoryInfo tjdDInfo = new DirectoryInfo(Path.Combine(packDInfo.FullName, Constants.DirectoryName.tjd));
                // TJDのエクスポート
                if (isTJDEnabled) {
                    if (isTJDCombine) {
                        // TODO 一緒くたにするやつ
                    } else {
                        this.TJDRed.Write(tjdDInfo.FullName, numberedTJCName);
                        this.TJDGold.Write(tjdDInfo.FullName, numberedTJCName);
                    }
                }
                return true;
            }
            catch (Exception ex) {
                MessageBox.Show($"TJCの出力に失敗しました\r\n" +
                                $"コース名: {this.Name}\r\n" +
                                $"{ex.Message}", "ざんねん", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

        }
    }
}
