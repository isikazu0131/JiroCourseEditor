using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace JiroCourseEditor {
    public partial class Form1 : Form {
        /// <summary>
        /// 設定情報
        /// </summary>
        private Setting setting;

        /// <summary>
        /// エラーダイアログ
        /// </summary>
        private ErrorDialog errorDialog;

        /// <summary>
        /// TJAをD&Dした際にどこに格納するかを質問するダイアログ
        /// </summary>
        private SelectTJAIndex selectTJAIndexDialog;

        /// <summary>
        /// 現在開いているパックファイル名（完全パス）
        /// </summary>
        private string NowOpeningFilePath = "";

        /// <summary>
        /// 起動中に扱うコースパック
        /// </summary>
        private TJP nowTJP;

        /// <summary>
        /// 起動中に扱うTJC
        /// </summary>
        private TJC nowTJC;

        /// <summary>
        /// TJC内のtja(5曲まで)
        /// </summary>
        private TJA[] TJAs = new TJA[5];

        /// <summary>
        /// テキストボックスが実際に変更されたか
        /// ※選択ノード変更によるテキストボックス変更でないか
        /// </summary>
        private bool isTextChangeing = false;

        /// <summary>
        /// 閾値変更用フラグ
        /// </summary>
        private bool FlagThresholdEdit = false;

        /// <summary>
        /// パック名TextBox編集用フラグ
        /// </summary>
        private bool FlagTbPackNameEdit = false;

        /// <summary>
        /// 現在のファイルが保存されているか
        /// </summary>
        private bool isSaved = true;

        /// <summary>
        /// 現在選択しているTJCが何番目か
        /// </summary>
        private int nowTJCindex = 0;

        /// <summary>
        /// テストプレイ時に生成されたTJC
        /// </summary>
        private List<FileInfo> TestTJCs = new List<FileInfo>();

        public Form1() {
            InitializeComponent();
        }

        /// <summary>
        /// フォーム読み込み
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_Load(object sender, EventArgs e) {
            setting = Setting.Read();
            CbIsTestTJCDelete.Checked = setting.IsTestTJCDelete;
            errorDialog = new ErrorDialog();
            selectTJAIndexDialog = new SelectTJAIndex();
            nowTJP = new TJP();
            nowTJP.Name = Constants.TJPDefault.Name;

            nowTJP.PackFolderBackColor = ColorInfo.GetColorCode(Constants.TJPDefault.PackBackColor);
            nowTJP.PackFolderForeColor = ColorInfo.GetColorCode(Constants.TJPDefault.PackForeColor);
            nowTJP.CourseFolderBackColor = ColorInfo.GetColorCode(Constants.TJPDefault.CourseBackColor);
            nowTJP.CourseFolderForeColor = ColorInfo.GetColorCode(Constants.TJPDefault.CourseForeColor);
            nowTJP.SongFolderBackColor = ColorInfo.GetColorCode(Constants.TJPDefault.SongBackColor);
            nowTJP.SongFolderForeColor = ColorInfo.GetColorCode(Constants.TJPDefault.SongForeColor);

            this.Text = $"{nowTJP.Name}{Constants.Extention.TJP} - {Constants.AppInfo.Name} {Constants.AppInfo.Version}";
            this.AllowDrop = true;
            this.MaximumSize = this.Size;
            this.MinimumSize = this.Size;

            // これだけ初期値を設定する
            CbRedOrGold.SelectedIndex = 0;

            PanelPack.Visible = true;
            PanelPack.Enabled = true;
            PanelCourse.Visible = false;
            PanelCourse.Enabled = false;
        }

        /// <summary>
        /// 現在のフォームのタイトルを変更します
        /// </summary>
        private void FormTitleChange() {
            // 変更内容の保存状態も書いておきたい
            string strIsSaved = isSaved ? "" : "(変更内容未保存)";
            // 開いているパスのファイル名部分だけ
            string path = Path.GetFileName(NowOpeningFilePath);
            // 新規作成等で選択していない場合はパックノードを選択状態にする
            if (TrPack.SelectedNode == null) {
                TrPack.SelectedNode = TrPack.Nodes[0];
            }
            if (TrPack.SelectedNode.Level == 0) {
                // 当アプリを起動した時の初期状態
                if (String.IsNullOrEmpty(NowOpeningFilePath)) {
                    this.Text = $"{nowTJP.Name}{Constants.Extention.TJP}{strIsSaved} - {Constants.AppInfo.Name} {Constants.AppInfo.Version}";
                } else {
                    this.Text = $"{path}{strIsSaved} - {Constants.AppInfo.Name} {Constants.AppInfo.Version}";
                }
            } else if (TrPack.SelectedNode.Level == 1) {
                // 当アプリを起動した時の初期状態
                if (String.IsNullOrEmpty(NowOpeningFilePath)) {
                    this.Text = $"{nowTJP.Name}{Constants.Extention.TJP}{strIsSaved} - {nowTJC.Name} - {Constants.AppInfo.Name} {Constants.AppInfo.Version}";
                } else {
                    this.Text = $"{path}{strIsSaved} - {nowTJC.Name} - {Constants.AppInfo.Name} {Constants.AppInfo.Version}";
                }
            } else {
                //何もしない
            }
        }

        /// <summary>
        /// 選択されているノードをもとに表示する画面を変更する
        /// </summary>
        private void ChangeMenuByTrTJPSelectedNode(TreeNode selectedNode) {
            switch (selectedNode.Level) {
                // パック名選択時
                case 0:
                    TrPack.SelectedNode = selectedNode;

                    // 右クリックメニューモード変更
                    TrPack.ContextMenuStrip = CmsPack;
                    PanelPack.Enabled = true;
                    PanelPack.Visible = true;
                    PanelCourse.Enabled = false;
                    PanelCourse.Visible = false;

                    LbTJCCount.Text = nowTJP.TJCs.Count().ToString();
                    LbTJPMapsCount.Text = nowTJP.TJCs.Sum(x => x.TJAs.Count(y => y != null)).ToString();
                    LbTotalPlayTime.Text = ToMinSec(nowTJP.TJCs.Sum(x => x.TotalOggTime()));
                    LbPackSize.Text = (nowTJP.TJCs.Sum(x => (double)x.GetTJAsSize()) / 1024 / 1024).ToString("0.00") + " MB";

                    // 内部から変更するためTbPackName変更メソッドは通らない
                    FlagTbPackNameEdit = true;
                    TbPackName.Text = nowTJP.Name;
                    FlagTbPackNameEdit = false;

                    FormTitleChange();
                    break;
                // コース名選択時
                case 1:
                    TrPack.SelectedNode = selectedNode;

                    // 一時的にテキストボックスの変更モードを選択ノード変更によるモードに移す
                    isTextChangeing = false;
                    nowTJCindex = TrPack.SelectedNode.Index;
                    nowTJC = nowTJP.TJCs[nowTJCindex];

                    // 右クリックメニューモード変更
                    TrPack.ContextMenuStrip = CmsCourse;
                    TbCourseName.Text = nowTJP.TJCs[nowTJCindex].Name;
                    TJAs = nowTJP.TJCs[nowTJCindex].TJAs.ToArray();
                    SetTJCToView(false);
                    PanelPack.Enabled = false;
                    PanelPack.Visible = false;
                    PanelCourse.Enabled = true;
                    PanelCourse.Visible = true;

                    // テキストボックスの変更モードを元に戻す
                    isTextChangeing = true;

                    FormTitleChange();
                    break;
                // TJA名選択時
                case 2:
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// パックツリーを選択した際の処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TrPack_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e) {
            ChangeMenuByTrTJPSelectedNode(e.Node);
        }

        /// <summary>
        /// パック名変更
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TbPackName_TextChanged(object sender, EventArgs e) {
            if (FlagTbPackNameEdit == true) return;
            var PackNode = TrPack.SelectedNode;
            if (PackNode != null) {
                PackNode.Text = TbPackName.Text;
                nowTJP.Name = TbPackName.Text;
                isSaved = false;
                FormTitleChange();
            }
        }

        /// <summary>
        /// コース名変更
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TbCourseName_TextChanged(object sender, EventArgs e) {
            if (!isTextChangeing) return;
            if (TrPack.SelectedNode.Level == 0) return;
            var PackNode = TrPack.SelectedNode;
            if (PackNode != null) {
                PackNode.Text = TbCourseName.Text;
                NmNumbering.Value = GetNumForCourseNum();

                nowTJP.TJCs[nowTJCindex].Name = TbCourseName.Text;
                nowTJP.TJCs[nowTJCindex].Number = (int)NmNumbering.Value;
            }
            isSaved = false;
            FormTitleChange();
        }

        /// <summary>
        /// コース名から出来るだけナンバリングを自動設定します(30まで)
        /// </summary>
        /// <returns></returns>
        private int GetNumForCourseNum() {
            // コース名を取得しておく
            string CName = TbCourseName.Text;
            if (CName.Contains("30") || CName.Contains("３０") || CName.Contains("三十")   || CName.Contains("参拾")   || CName.Contains("参拾")) return 30;
            if (CName.Contains("29") || CName.Contains("２９") || CName.Contains("二十九") || CName.Contains("弐拾九") || CName.Contains("弐拾玖")) return 29;
            if (CName.Contains("28") || CName.Contains("２８") || CName.Contains("二十八") || CName.Contains("弐拾八") || CName.Contains("弐拾捌")) return 28;
            if (CName.Contains("27") || CName.Contains("２７") || CName.Contains("二十七") || CName.Contains("弐拾七") || CName.Contains("弐拾漆")) return 27;
            if (CName.Contains("26") || CName.Contains("２６") || CName.Contains("二十六") || CName.Contains("弐拾六") || CName.Contains("弐拾陸")) return 26;
            if (CName.Contains("25") || CName.Contains("２５") || CName.Contains("二十五") || CName.Contains("弐拾五") || CName.Contains("弐拾伍")) return 25;
            if (CName.Contains("24") || CName.Contains("２４") || CName.Contains("二十四") || CName.Contains("弐拾四") || CName.Contains("弐拾肆")) return 24;
            if (CName.Contains("23") || CName.Contains("２３") || CName.Contains("二十三") || CName.Contains("弐拾参") || CName.Contains("弐拾参")) return 23;
            if (CName.Contains("22") || CName.Contains("２２") || CName.Contains("二十二") || CName.Contains("弐拾弐") || CName.Contains("弐拾弐")) return 22;
            if (CName.Contains("21") || CName.Contains("２１") || CName.Contains("二十一") || CName.Contains("弐拾壱") || CName.Contains("弐拾壱")) return 21;
            if (CName.Contains("20") || CName.Contains("２０") || CName.Contains("二十")   || CName.Contains("弐拾")   || CName.Contains("弐拾")) return 20;
            if (CName.Contains("19") || CName.Contains("１９") || CName.Contains("十九")   || CName.Contains("拾九")   || CName.Contains("拾玖")) return 19;
            if (CName.Contains("18") || CName.Contains("１８") || CName.Contains("十八")   || CName.Contains("拾八")   || CName.Contains("拾捌")) return 18;
            if (CName.Contains("17") || CName.Contains("１７") || CName.Contains("十七")   || CName.Contains("拾七")   || CName.Contains("拾漆")) return 17;
            if (CName.Contains("16") || CName.Contains("１６") || CName.Contains("十六")   || CName.Contains("拾六")   || CName.Contains("拾陸")) return 16;
            if (CName.Contains("15") || CName.Contains("１５") || CName.Contains("十五")   || CName.Contains("拾五")   || CName.Contains("拾伍")) return 15;
            if (CName.Contains("14") || CName.Contains("１４") || CName.Contains("十四")   || CName.Contains("拾四")   || CName.Contains("拾肆") || CName.Contains("達人")) return 14;
            if (CName.Contains("13") || CName.Contains("１３") || CName.Contains("十三")   || CName.Contains("拾参")   || CName.Contains("超人")) return 13;
            if (CName.Contains("12") || CName.Contains("１２") || CName.Contains("十二")   || CName.Contains("拾弐")   || CName.Contains("名人")) return 12;
            if (CName.Contains("11") || CName.Contains("１１") || CName.Contains("十一")   || CName.Contains("拾壱")   || CName.Contains("玄人")) return 11;
            if (CName.Contains("10") || CName.Contains("１０") || CName.Contains("十")     || CName.Contains("拾")     || CName.Contains("拾")) return 10;
            if (CName.Contains("9")  || CName.Contains("９")   || CName.Contains("九")     || CName.Contains("九")     || CName.Contains("玖")) return 9;
            if (CName.Contains("8")  || CName.Contains("８")   || CName.Contains("八")     || CName.Contains("八")     || CName.Contains("捌")) return 8;
            if (CName.Contains("7")  || CName.Contains("７")   || CName.Contains("七")     || CName.Contains("七")     || CName.Contains("漆")) return 7;
            if (CName.Contains("6")  || CName.Contains("６")   || CName.Contains("六")     || CName.Contains("六")     || CName.Contains("陸")) return 6;
            if (CName.Contains("5")  || CName.Contains("５")   || CName.Contains("五")     || CName.Contains("五")     || CName.Contains("伍")) return 5;
            if (CName.Contains("4")  || CName.Contains("４")   || CName.Contains("四")     || CName.Contains("四")     || CName.Contains("肆")) return 4;
            if (CName.Contains("3")  || CName.Contains("３")   || CName.Contains("三")     || CName.Contains("参")     || CName.Contains("参")) return 3;
            if (CName.Contains("2")  || CName.Contains("２")   || CName.Contains("二")     || CName.Contains("弐")     || CName.Contains("弐")) return 2;
            if (CName.Contains("1")  || CName.Contains("１")   || CName.Contains("一")     || CName.Contains("壱")     || CName.Contains("初")) return 1;
            return 1;

        }

        /// <summary>
        /// 新しいコースを追加する際に「新しいコース」が段位内にいくつあるかをカウントし、
        /// コース名に格納できるように加工します
        /// </summary>
        /// <returns></returns>
        private string GetNewCourseNum() {
            List<TreeNode> nodes = new List<TreeNode>();
            foreach (TreeNode node in TrPack.SelectedNode.Nodes) {
                nodes.Add(node);
            }
            int num = nodes.Count(x => x.Text.Contains("新しいコース")) + 1;

            return $"({num})";
        }

        /// <summary>
        /// パックにTJCを追加します
        /// </summary>
        private void AddNewTJC(string TITLE) {
            TJC tjc = new TJC();
            tjc.Name = TITLE;
            nowTJP.TJCs.Add(tjc);
            nowTJC = tjc;
            isSaved = false;
            FormTitleChange();
            // コース数だけ変える
            LbTJCCount.Text = nowTJP.TJCs.Count().ToString();
        }

        /// <summary>
        /// コースを削除する
        /// </summary>
        private void DeleteTJC() {
            if (MessageBox.Show("選択したコースを削除しますか？", "ホンマに消すんか？", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes) {
                TrPack.SelectedNode.Remove();
                nowTJP.TJCs.RemoveAt(nowTJCindex);
                if (nowTJP.TJCs.Count == 0) {
                    PanelPack.Enabled = true;
                    PanelPack.Visible = true;
                    PanelCourse.Enabled = false;
                    PanelCourse.Visible = false;
                } else {
                    nowTJCindex = TrPack.SelectedNode.Index;
                    nowTJC = nowTJP.TJCs[nowTJCindex];
                    TJAs = nowTJC.TJAs.ToArray();
                    SetTJCToView(false);
                }
                isSaved = false;
                FormTitleChange();
            }
        }

        /// <summary>
        /// 現在のTJPをTreeViewに再描画します
        /// </summary>
        private void DrawTreeViewNowTJP() {
            TrPack.Nodes.Clear();
            // ここからは読み込んだTJPの内容をTreeViewに描写する作業
            TrPack.Nodes.Add(nowTJP.Name);
            foreach (var tjc in nowTJP.TJCs) {
                TrPack.Nodes[0].Nodes.Add(tjc.Name);
            }
            FormTitleChange();
        }

        /// <summary>
        /// 選択中のTJC内のTJAリストに格納する
        /// </summary>
        /// <param name="TJAFInfo"></param>
        private void SetTJAtoTJAListView(FileInfo TJAFInfo, int index) {
            TJA tja = new TJA(TJAFInfo);
            TJAs[index] = tja;
            //// 現在のTJC内TJAリストで一番最初に空になっている箇所に入れる
            //foreach(var t in TJAs.Select((v, i) => (v, i))) {
            //    if (t.v == null) {
            //        TJAs[t.i] = tja;
            //        break;
            //    }
            //    if (String.IsNullOrEmpty(t.v.TJAPath.Name)) {
            //        TJAs[t.i] = tja;
            //        break;
            //    }
            //}
            nowTJP.TJCs[TrPack.SelectedNode.Index].TJAs = TJAs.ToList();
            nowTJC = nowTJP.TJCs[TrPack.SelectedNode.Index];
            SetTJCToView(false);
            FormTitleChange();
        }

        /// <summary>
        /// 現在のTJP（パック情報）を画面上に表示します
        /// </summary>
        private void SetTJPInfoToView() {
            TbPackName.Text = nowTJP.Name;
            LbPackColorView.BackColor = ColorInfo.GetColor(nowTJP.PackFolderBackColor);
            LbPackColorView.ForeColor = ColorInfo.GetColor(nowTJP.PackFolderForeColor);
            LbCourseColorView.BackColor = ColorInfo.GetColor(nowTJP.CourseFolderBackColor);
            LbCourseColorView.ForeColor = ColorInfo.GetColor(nowTJP.CourseFolderForeColor);
            LbSongColorView.BackColor = ColorInfo.GetColor(nowTJP.SongFolderBackColor);
            LbSongColorView.ForeColor = ColorInfo.GetColor(nowTJP.SongFolderForeColor);

            // TJP情報
            LbTJPMapsCount.Text = nowTJP.TJCs.Sum(x => x.TJAs.Count(y => y != null)).ToString();
            LbTotalPlayTime.Text = nowTJP.TJCs.Sum(x => x.TJAs.Where(y => y != null).Sum(y => y.MusicPlayTime)).ToString();
        }

        /// <summary>
        /// 現在のTJCを画面上に表示します
        /// </summary>
        private void SetTJCToView(bool isCreatedNewTJC) {
            TbTJA1.Text = "";
            LbNotesCount1.Text = "0";
            LbLevel1.Text = "0.0";
            TbTJA2.Text = "";
            LbNotesCount2.Text = "0";
            LbLevel2.Text = "0.0";
            TbTJA3.Text = "";
            LbNotesCount3.Text = "0";
            LbLevel3.Text = "0.0";
            TbTJA4.Text = "";
            LbNotesCount4.Text = "0";
            LbLevel4.Text = "0.0"; 
            TbTJA5.Text = "";
            LbNotesCount5.Text = "0";
            LbLevel5.Text = "0.0";
            foreach (var t in TJAs.Select((v, i) => (v, i))) {
                // nullが入っていることもあるが、そのあとにもtjaが格納されていることも
                // なくはないので一旦飛ばす
                if (t.v == null) continue;
                switch (t.i) {
                    case 0:
                        TbTJA1.Text = t.v.TITLE;
                        LbNotesCount1.Text = t.v.NotesCount.ToString();
                        LbLevel1.Text = t.v.LEVEL.ToString("0.0");
                        break;
                    case 1:
                        TbTJA2.Text = t.v.TITLE;
                        LbNotesCount2.Text = t.v.NotesCount.ToString();
                        LbLevel2.Text = t.v.LEVEL.ToString("0.0");
                        break;
                    case 2:
                        TbTJA3.Text = t.v.TITLE;
                        LbNotesCount3.Text = t.v.NotesCount.ToString();
                        LbLevel3.Text = t.v.LEVEL.ToString("0.0");
                        break;
                    case 3:
                        TbTJA4.Text = t.v.TITLE;
                        LbNotesCount4.Text = t.v.NotesCount.ToString();
                        LbLevel4.Text = t.v.LEVEL.ToString("0.0");
                        break;
                    case 4:
                        TbTJA5.Text = t.v.TITLE;
                        LbNotesCount5.Text = t.v.NotesCount.ToString();
                        LbLevel5.Text = t.v.LEVEL.ToString("0.0");
                        break;
                    default:
                        break;
                }
            }

            // 画面上のUIを更新する
            // ナンバリング更新
            NmNumbering.Value = GetNumForCourseNum();

            // フォルダカラー
            LbLevelColorView.BackColor = ColorInfo.GetColor(nowTJC.LevelBackColor);
            LbLevelColorView.ForeColor = ColorInfo.GetColor(nowTJC.LevelForeColor);

            // 新規作成したTJCなら初期配置にする
            if (isCreatedNewTJC) {
                // 赤合格へ切り替え
                CbRedOrGold.SelectedIndex = 0;
                CbCondition1.SelectedIndex = 7;
                CbCondition2.SelectedIndex = 7;
                CbCondition3.SelectedIndex = 7;
            }
            // 合格条件名称表示切り替え
            // 表示前は-1
            if(CbRedOrGold.SelectedIndex == 0) {
                TbConditionName.Text = nowTJC.TJDRed.Name;
            }else if(CbRedOrGold.SelectedIndex == 1) {
                TbConditionName.Text = nowTJC.TJDGold.Name;
            }

            if (nowTJC.TJDRed != null) {
                CbCondition1.SelectedIndex = (int)nowTJC.TJDRed.PassingConditions[0].passingType;
                NmThreshold1.Value = nowTJC.TJDRed.PassingConditions[0].Threshold;
                NmRatio1.Value = (decimal)nowTJC.TJDRed.PassingConditions[0].Ratio * 100;
                CbCondition2.SelectedIndex = (int)nowTJC.TJDRed.PassingConditions[1].passingType;
                NmThreshold2.Value = nowTJC.TJDRed.PassingConditions[1].Threshold;
                NmRatio2.Value = (decimal)nowTJC.TJDRed.PassingConditions[1].Ratio * 100;
                CbCondition3.SelectedIndex = (int)nowTJC.TJDRed.PassingConditions[2].passingType;
                NmThreshold3.Value = nowTJC.TJDRed.PassingConditions[2].Threshold;
                NmRatio3.Value = (decimal)nowTJC.TJDRed.PassingConditions[2].Ratio * 100;
            }
            UpdateThresholdByRatio();
            CbUseCondition.Checked = nowTJC.isTJDEnabled;
            TbCourseName.Text = nowTJC.Name;
            LbCourseNotes.Text = nowTJC.TotalNoteCount().ToString();
            LbCourseTime.Text = ToMinSec(nowTJC.TotalOggTime());
            LbAvgLEVEL.Text = nowTJC.AvgLevel().ToString("0.0");

        }

        /// <summary>
        /// 秒を分秒にします
        /// </summary>
        /// <param name="seconds"></param>
        /// <returns></returns>
        private string ToMinSec(double seconds) {
            if (seconds < 0) { return "取得エラー"; }
            seconds = Math.Round(seconds, 3);
            int minutes = (int)(seconds / 60);
            double remainingSeconds = seconds - minutes * 60.0;
            return $"{minutes}:{remainingSeconds:00.000}";
        }

        /// <summary>
        /// 当アプリへドラッグアンドドロップされた際の処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_DragDrop(object sender, DragEventArgs e) {
            string[] Files = null;
            // ファイルの場合のみ処理を行う
            if (e.Data.GetDataPresent(DataFormats.FileDrop)) {
                Files = (string[])e.Data.GetData(DataFormats.FileDrop);
                if (Files.Length >= 2) {
                    errorDialog.ViewDialog("ファイルは１つだけドラッグアンドドロップしてください");
                    return;
                }
            }
            if (TrPack.SelectedNode.Level == 0) {
                errorDialog.ViewDialog("追加するコースを選択した状態でドラッグアンドドロップしてください");
                return;
            }

            // D&Dしたファイルの情報を取得しておく(*.tja前提)
            FileInfo tjaInfo = new FileInfo(Files[0]);
            if (tjaInfo.Extension != Constants.Extention.tja && tjaInfo.Extension != Constants.Extention.TJA) {
                errorDialog.ViewDialog("ドラッグアンドドロップするファイルは必ず\".tja\"ファイルを指定してください");
                return;
            }

            // D&DされたtjaをTJC内に突っ込む
            if (selectTJAIndexDialog.ShowDialog() == DialogResult.OK) {
                // 何番目に突っ込むか
                int inputIndex = selectTJAIndexDialog.SelectedIndex;
                SetTJAtoTJAListView(tjaInfo, inputIndex);
            }
        }

        #region IO関係
        /// <summary>
        /// 新規作成
        /// </summary>
        private void CreateNewFile() {
            nowTJP = new TJP();
            nowTJP.Name = Constants.TJPDefault.Name;
            isSaved = true;
            DrawTreeViewNowTJP();
            TrPack.SelectedNode = TrPack.Nodes[0];

            ChangeMenuByTrTJPSelectedNode(TrPack.Nodes[0]);
        }

        /// <summary>
        /// パックファイルを開く
        /// </summary>
        private void OpenPackFile() {
            var openedTJP = TJP.Read(NowOpeningFilePath);
            if (openedTJP == null) {
                MessageBox.Show("ファイルの読み込みに失敗しました。");
                return;
            }
            nowTJP = openedTJP;
            DrawTreeViewNowTJP();
            TrPack.SelectedNode = TrPack.Nodes[0];
            ChangeMenuByTrTJPSelectedNode(TrPack.Nodes[0]);
            TrPack.ExpandAll();
            isSaved = true;
        }

        /// <summary>
        /// TJPを上書きか新規保存のどっちかで保存する
        /// </summary>
        /// <returns>true: 保存に成功 false: 保存に失敗</returns>
        private bool SaveTJP() {
            bool isSaveSuccess = false;
            // 新規作成したものなら新規に名前を付けて保存する
            if (String.IsNullOrEmpty(NowOpeningFilePath)) {
                isSaveSuccess = SaveNewTJPFile();
            } else {
                isSaveSuccess = SaveTJPFile();
            }
            return isSaveSuccess;
        }

        /// <summary>
        /// TJPファイルを保存する（上書き）
        /// </summary>
        /// <returns></returns>
        private bool SaveTJPFile() {
            var isSaveSuccess = TJP.Write(nowTJP, NowOpeningFilePath);
            if (isSaveSuccess) {
                isSaved = true;
                return true;
            } else {
                return false;
            }
        }

        /// <summary>
        /// TJPファイルを保存する（新規）
        /// </summary>
        private bool SaveNewTJPFile() {
            CommonSaveFileDialog sFileDialog = new CommonSaveFileDialog();
            sFileDialog.Title = "名前を付けて保存";
            sFileDialog.Filters.Add(new CommonFileDialogFilter("太鼓さん次郎向けパック編集用ファイル", "*.tjp"));
            sFileDialog.DefaultFileName = $"{nowTJP.Name}{Constants.Extention.TJP}";
            if (sFileDialog.ShowDialog() == CommonFileDialogResult.Ok) {
                // 保存時のファイルパス
                string sFileName = sFileDialog.FileName;
                // .tjpを拡張子として設定していなかった場合は付け足す
                if (sFileName.EndsWith(Constants.Extention.TJP) == false) {
                    sFileName += Constants.Extention.TJP;
                }
                var isSaveSuccess = TJP.Write(nowTJP, sFileName);
                if (isSaveSuccess) {
                    isSaved = true;
                    NowOpeningFilePath = sFileDialog.FileName;
                    return true;
                } else {
                    return false;
                }
            } else {
                return false;
            }
        }

        /// <summary>
        /// 変更が未保存だった場合の確認処理
        /// </summary>
        /// <returns>true : 既に保存済み or ユーザ側が保存した / false : ユーザ側がキャンセルした</returns>
        private bool SaveCheck() {
            if (isSaved == false) {
                var saveAnswer = MessageBox.Show($"パックファイル内容の変更が保存されていません。\r\n保存しますか？",
                                                 "編集内容がパーになるとこだったよ！！！！あぶねー",
                                                 MessageBoxButtons.YesNoCancel,
                                                 MessageBoxIcon.Warning);
                DialogResult moreAnswer = DialogResult.No;
                if (saveAnswer == DialogResult.No) {
                    moreAnswer = MessageBox.Show("本当に保存しなくて大丈夫？",
                                                     "本当に大丈夫？",
                                                     MessageBoxButtons.YesNo,
                                                     MessageBoxIcon.Warning);
                }
                if (saveAnswer == DialogResult.Yes || moreAnswer == DialogResult.No) {
                    // 保存操作
                    bool isSaveSuccess = SaveTJP();
                    // 保存した場合
                    if (isSaveSuccess) { return true; }
                    // 保存できない、ユーザ側がキャンセル
                    else return false;
                } else {
                    return true;
                }
            }
            return true;
        }
        #endregion

        /// <summary>
        /// ドラッグ中のステータス変更
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_DragEnter(object sender, DragEventArgs e) {
            // ファイルの場合のみ処理を行う
            if (e.Data.GetDataPresent(DataFormats.FileDrop)) {
                e.Effect = DragDropEffects.Copy;
            } else {
                e.Effect = DragDropEffects.None;
            }
        }

        #region 画面上部メニュー

        /// <summary>
        /// 新規作成
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void 新規作成ToolStripMenuItem_Click(object sender, EventArgs e) {
            // ファイルの変更の未保存を確認する
            if (SaveCheck() == true) {
                CreateNewFile();
            }
        }

        /// <summary>
        /// パックファイルを開く
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void 開くToolStripMenuItem_Click(object sender, EventArgs e) {
            // ファイルの変更の未保存を確認する
            if (SaveCheck() == true) {
                // 開くファイルを取得する
                CommonOpenFileDialog fDialog = new CommonOpenFileDialog();
                fDialog.Title = "開くパックファイルを選択";
                fDialog.Multiselect = false;
                fDialog.Filters.Add(new CommonFileDialogFilter("パックエディット用ファイル", "*.tjp"));
                if (fDialog.ShowDialog() == CommonFileDialogResult.Ok) {
                    NowOpeningFilePath = fDialog.FileName;
                    OpenPackFile();
                }
            }
        }

        /// <summary>
        /// 上書き保存処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void 上書き保存ToolStripMenuItem_Click(object sender, EventArgs e) {
            SaveTJP();
        }

        /// <summary>
        /// 名前を付けて保存処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void 名前を付けて保存ToolStripMenuItem_Click(object sender, EventArgs e) {
            SaveNewTJPFile();
        }

        /// <summary>
        /// パック出力フォルダ設定
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void パックを出力するフォルダの設定ToolStripMenuItem_Click(object sender, EventArgs e) {
            var folderDialog = new CommonOpenFileDialog();
            folderDialog.IsFolderPicker = true;

            if (folderDialog.ShowDialog() == CommonFileDialogResult.Ok) {
                setting.PackOutputFolderName = folderDialog.FileName;
                Setting.Write(setting);
            } else {
                return;
            }
        }

        #endregion

        #region 課題曲選択ボタン
        /// <summary>
        /// 選択したtjaをtjcにセットします
        /// </summary>
        /// <param name="tjaindex"></param>
        private void SetTJA(int tjaindex) {
            var openFDialog = new CommonOpenFileDialog();
            openFDialog.Title = "譜面を選択してください";
            openFDialog.Filters.Add(new CommonFileDialogFilter("tjaファイル", "*.tja"));
            if (openFDialog.ShowDialog() == CommonFileDialogResult.Ok) {
                TJA tja = TJA.SetTJAbyPath(openFDialog.FileName);
                if (tja == null) {
                    MessageBox.Show("tjaファイルを登録できませんでした。");
                    return;
                }
                TJAs[tjaindex] = tja;
                nowTJP.TJCs[nowTJCindex].TJAs = TJAs.ToList();
                SetTJCToView(false);
            }
        }

        private void BtSongSelect1_Click(object sender, EventArgs e) {
            SetTJA(0);
        }

        private void BtSongSelect2_Click(object sender, EventArgs e) {
            SetTJA(1);
        }

        private void BtSongSelect3_Click(object sender, EventArgs e) {
            SetTJA(2);
        }

        private void BtSongSelect4_Click(object sender, EventArgs e) {
            SetTJA(3);
        }

        private void BtSongSelect5_Click(object sender, EventArgs e) {
            SetTJA(4);
        }
        #endregion

        #region クリアボタン
        private void BtClear1_Click(object sender, EventArgs e) {
            nowTJP.TJCs[TrPack.SelectedNode.Index].TJAs[0] = null;
            TJAs = nowTJP.TJCs[TrPack.SelectedNode.Index].TJAs.ToArray();
            SetTJCToView(false);
        }

        private void BtClear2_Click(object sender, EventArgs e) {
            nowTJP.TJCs[TrPack.SelectedNode.Index].TJAs[1] = null;
            TJAs = nowTJP.TJCs[TrPack.SelectedNode.Index].TJAs.ToArray();
            SetTJCToView(false);
        }

        private void BtClear3_Click(object sender, EventArgs e) {
            nowTJP.TJCs[TrPack.SelectedNode.Index].TJAs[2] = null;
            TJAs = nowTJP.TJCs[TrPack.SelectedNode.Index].TJAs.ToArray();
            SetTJCToView(false);
        }

        private void BtClear4_Click(object sender, EventArgs e) {
            nowTJP.TJCs[TrPack.SelectedNode.Index].TJAs[3] = null;
            TJAs = nowTJP.TJCs[TrPack.SelectedNode.Index].TJAs.ToArray();
            SetTJCToView(false);
        }

        private void BtClear5_Click(object sender, EventArgs e) {
            nowTJP.TJCs[TrPack.SelectedNode.Index].TJAs[4] = null;
            TJAs = nowTJP.TJCs[TrPack.SelectedNode.Index].TJAs.ToArray();
            SetTJCToView(false);
        }

        #endregion

        /// <summary>
        /// 右クリックした際に右クリックしたノードを選択状態にする
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TrPack_MouseDown(object sender, MouseEventArgs e) {
            if (e.Button == MouseButtons.Right) {
                var clickNode = TrPack.GetNodeAt(e.X, e.Y);
                if (clickNode != null) {
                    TrPack.SelectedNode = clickNode;
                }
            }
        }

        #region Genre.iniカラー変更
        /// <summary>
        /// パックのフォルダカラー変更
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtChangePackColor_Click(object sender, EventArgs e) {
            if (colorDialog.ShowDialog() == DialogResult.OK) {
                LbPackColorView.BackColor = colorDialog.Color;
            }
            if (colorDialog.ShowDialog() == DialogResult.OK) {
                LbPackColorView.ForeColor = colorDialog.Color;
            }
            nowTJP.PackFolderBackColor = ColorInfo.GetColorCode(LbPackColorView.BackColor);
            nowTJP.PackFolderForeColor = ColorInfo.GetColorCode(LbPackColorView.ForeColor);
            isSaved = false;
            SaveTJPFile();
        }
        /// <summary>
        /// TJCフォルダカラー変更
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtChangeCourseColor_Click(object sender, EventArgs e) {
            if (colorDialog.ShowDialog() == DialogResult.OK) {
                LbCourseColorView.BackColor = colorDialog.Color;
            }
            if (colorDialog.ShowDialog() == DialogResult.OK) {
                LbCourseColorView.ForeColor = colorDialog.Color;
            }
            nowTJP.CourseFolderBackColor = ColorInfo.GetColorCode(LbCourseColorView.BackColor);
            nowTJP.CourseFolderForeColor = ColorInfo.GetColorCode(LbCourseColorView.ForeColor);
            isSaved = false;
            SaveTJPFile();
        }
        /// <summary>
        /// 課題曲フォルダカラー変更
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtChangeSongColor_Click(object sender, EventArgs e) {
            if (colorDialog.ShowDialog() == DialogResult.OK) {
                LbSongColorView.BackColor = colorDialog.Color;
            }
            if (colorDialog.ShowDialog() == DialogResult.OK) {
                LbSongColorView.ForeColor = colorDialog.Color;
            }
            nowTJP.SongFolderBackColor = ColorInfo.GetColorCode(LbSongColorView.BackColor);
            nowTJP.SongFolderForeColor = ColorInfo.GetColorCode(LbSongColorView.ForeColor);
            isSaved = false;
            SaveTJPFile();
        }
        /// <summary>
        /// 課題曲フォルダ内レベル別フォルダカラー変更
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtChangeCourseSongColor_Click(object sender, EventArgs e) {
            if (colorDialog.ShowDialog() == DialogResult.OK) {
                LbLevelColorView.BackColor = colorDialog.Color;
            }
            if (colorDialog.ShowDialog() == DialogResult.OK) {
                LbLevelColorView.ForeColor = colorDialog.Color;
            }
            nowTJP.TJCs[nowTJCindex].LevelBackColor = ColorInfo.GetColorCode(LbLevelColorView.BackColor);
            nowTJP.TJCs[nowTJCindex].LevelForeColor = ColorInfo.GetColorCode(LbLevelColorView.ForeColor);
            isSaved = false;
        }

        #endregion

        #region 右クリックメニュー
        /// <summary>
        /// コースの削除ボタン押下
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RClickMenuTJCDelete_Click(object sender, EventArgs e) {
            DeleteTJC();
        }

        /// <summary>
        /// 新しいコースの追加ボタン押下
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RClickMenuAddNewTJC_Click(object sender, EventArgs e) {
            string NewCourseNum = GetNewCourseNum();
            TrPack.Nodes[0].Nodes.Add($"新しいコース {NewCourseNum}");
            AddNewTJC($"新しいコース {NewCourseNum}");
            TrPack.ExpandAll();
        }

        #endregion

        #region フォーム関係
        /// <summary>
        /// フォームを閉じようとしたタイミングの処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_FormClosing(object sender, FormClosingEventArgs e) {
            // ファイルの変更の未保存を防ぐ
            // ユーザ側がキャンセルした場合
            if (SaveCheck() == false) {
                e.Cancel = true;
            }

            if (setting.IsTestTJCDelete) {
                foreach (var tjc in TestTJCs) {
                    tjc.Delete();
                }
            }

        }

        /// <summary>
        /// 上書き保存しようとしたかどうか検知して処理する
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_KeyDown(object sender, KeyEventArgs e) {
            if (e.Control && e.KeyCode == Keys.S) {
                SaveTJP();
            }
        }
        #endregion

        #region TJCの並び替え
        /// <summary>
        /// コースをナンバリング順で並べ替え
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RClickSortByNum_Click(object sender, EventArgs e) {
            nowTJP.TJCs.Sort((x, y) => x.Number.CompareTo(y.Number));
            ChangeMenuByTrTJPSelectedNode(TrPack.Nodes[0]);
            DrawTreeViewNowTJP();
            TrPack.ExpandAll();
        }

        /// <summary>
        /// コースを名前順で並べ替え
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RClickSortByTJCName_Click(object sender, EventArgs e) {
            nowTJP.TJCs.Sort((x, y) => x.Name.CompareTo(y.Name));
            ChangeMenuByTrTJPSelectedNode(TrPack.Nodes[0]);
            DrawTreeViewNowTJP();
            TrPack.ExpandAll();
        }

        #endregion

        private void ヘルプToolStripMenuItem_Click(object sender, EventArgs e) {
            MessageBox.Show("助けて～");
        }

        /// <summary>
        /// みおすな式段位風テンプレートを作るボタン押下
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtMJECreate_Click(object sender, EventArgs e) {
            if (SaveCheck() == true) {
                QFormSPDP qFormSPDP = new QFormSPDP();
                if (qFormSPDP.ShowDialog() == DialogResult.OK) {
                    TJP MDEtjp = TJP.CreateMJEtemp(qFormSPDP.isSP);
                    if (MDEtjp != null) {
                        nowTJP = MDEtjp;
                        DrawTreeViewNowTJP();
                        SetTJPInfoToView();
                        ChangeMenuByTrTJPSelectedNode(TrPack.Nodes[0]);
                        TrPack.ExpandAll();
                        MessageBox.Show("テンプレートを作成しました。");
                    }
                } else {
                    MessageBox.Show("テンプレートの作成をキャンセルしました。");
                }
            }
        }

        private void BtTJDforMJE_Click(object sender, EventArgs e) {

            // 赤合格の場合
            if (CbRedOrGold.SelectedIndex == 0) {
                // 各レベルによって条件を変える
                if (NmNumbering.Value >= 1 && NmNumbering.Value <= 5) {
                    NmRatio2.Value = (decimal)0.2;
                    NmRatio3.Value = (decimal)0.05;
                } else if (NmNumbering.Value >= 6 && NmNumbering.Value <= 10) {
                    NmRatio2.Value = (decimal)0.15;
                    NmRatio3.Value = (decimal)0.03;
                } else if (NmNumbering.Value >= 11) {
                    NmRatio2.Value = (decimal)0.1;
                    NmRatio3.Value = (decimal)0.02;
                }
            }
            // 金合格の場合
            else {
                // 各レベルによって条件を変える
                if (NmNumbering.Value >= 1 && NmNumbering.Value <= 5) {
                    NmRatio2.Value = (decimal)0.05;
                    NmRatio3.Value = (decimal)0.015;
                } else if (NmNumbering.Value >= 6 && NmNumbering.Value <= 10) {
                    NmRatio2.Value = (decimal)0.03;
                    NmRatio3.Value = (decimal)0.01;
                } else if (NmNumbering.Value >= 11) {
                    NmRatio2.Value = (decimal)0.02;
                    NmRatio3.Value = (decimal)0.005;
                }
            }

        }

        #region 条件設定
        /// <summary>
        /// 選択した条件によってUIを変更する
        /// </summary>
        /// <param name="cbCondition"></param>
        /// <param name="NmThreshold"></param>
        /// <param name="LbMoreLess"></param>
        /// <param name="NmRatio"></param>
        /// <param name="LbPer"></param>
        private void ConditionSetting(ComboBox cbCondition, NumericUpDown NmThreshold, Label LbMoreLess, NumericUpDown NmRatio, Label LbPer) {
            switch (cbCondition.SelectedItem) {
                case "なし":
                    NmThreshold.Enabled = false;
                    NmThreshold.Value = 0;
                    LbMoreLess.Text = "";
                    NmRatio.Value = 0;
                    NmRatio.Visible = false;
                    LbPer.Visible = false;
                    break;
                case "スコア":
                    NmThreshold.Enabled = true;
                    NmThreshold.Maximum = 99999999;
                    NmThreshold.Value = 1;
                    LbMoreLess.Text = "以上";
                    NmRatio.Value = 0;
                    NmRatio.Visible = false;
                    LbPer.Visible = false;
                    break;
                case "良の数":
                    NmThreshold.Enabled = true;
                    NmThreshold.Maximum = 65535 * 5;
                    NmThreshold.Value = 0;
                    LbMoreLess.Text = "以上";
                    NmRatio.Value = 0;
                    NmRatio.Visible = true;
                    LbPer.Visible = true;
                    break;
                case "可の数":
                    NmThreshold.Enabled = true;
                    NmThreshold.Maximum = 65535 * 5;
                    NmThreshold.Value = 0;
                    LbMoreLess.Text = "未満";
                    NmRatio.Value = 0;
                    NmRatio.Visible = true;
                    LbPer.Visible = true;
                    break;
                case "不可の数":
                    NmThreshold.Enabled = true;
                    NmThreshold.Maximum = 65535 * 5;
                    NmThreshold.Value = 0;
                    LbMoreLess.Text = "未満";
                    NmRatio.Value = 0;
                    NmRatio.Visible = true;
                    LbPer.Visible = true;
                    break;
                case "連打数":
                    NmThreshold.Enabled = true;
                    NmThreshold.Maximum = 99999;
                    NmThreshold.Value = 0;
                    LbMoreLess.Text = "以上";
                    NmRatio.Value = 0;
                    NmRatio.Visible = false;
                    LbPer.Visible = false;
                    break;
                case "最大コンボ数":
                    NmThreshold.Enabled = true;
                    NmThreshold.Maximum = 65535 * 5;
                    NmThreshold.Value = 0;
                    LbMoreLess.Text = "以上";
                    NmRatio.Value = 0;
                    NmRatio.Visible = false;
                    LbPer.Visible = false;
                    break;
                case "叩けた数":
                    NmThreshold.Enabled = true;
                    NmThreshold.Maximum = 65535 * 5;
                    NmThreshold.Value = 0;
                    LbMoreLess.Text = "以上";
                    NmRatio.Value = 0;
                    NmRatio.Visible = false;
                    LbPer.Visible = false;
                    break;
                default:
                    break;
            }
        }
        private void CbCondition1_SelectedIndexChanged(object sender, EventArgs e) {
            ConditionSetting(CbCondition1,
                             NmThreshold1,
                             LbMoreLess1,
                             NmRatio1,
                             LbPer1);
        }

        private void CbCondition2_SelectedIndexChanged(object sender, EventArgs e) {
            ConditionSetting(CbCondition2,
                             NmThreshold2,
                             LbMoreLess2,
                             NmRatio2,
                             LbPer2);
        }

        private void CbCondition3_SelectedIndexChanged(object sender, EventArgs e) {
            ConditionSetting(CbCondition3,
                             NmThreshold3,
                             LbMoreLess3,
                             NmRatio3,
                             LbPer3);
        }
        #endregion

        /// <summary>
        /// 合格条件切り替え
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CbRedOrGold_SelectedIndexChanged(object sender, EventArgs e) {
            if (nowTJC == null) return;
            // 赤合格
            if(CbRedOrGold.SelectedIndex == 0) {
                TbConditionName.Text = nowTJC.TJDRed.Name;
                CbCondition1.SelectedIndex = (int)nowTJC.TJDRed.PassingConditions[0].passingType;
                CbCondition2.SelectedIndex = (int)nowTJC.TJDRed.PassingConditions[1].passingType;
                CbCondition3.SelectedIndex = (int)nowTJC.TJDRed.PassingConditions[2].passingType;
                NmRatio1.Value = (decimal)(nowTJC.TJDRed.PassingConditions[0].Ratio * 100);
                NmRatio2.Value = (decimal)(nowTJC.TJDRed.PassingConditions[1].Ratio * 100);
                NmRatio3.Value = (decimal)(nowTJC.TJDRed.PassingConditions[2].Ratio * 100);
            } 
            // 金合格
            else {
                TbConditionName.Text = nowTJC.TJDGold.Name;
                CbCondition1.SelectedIndex = (int)nowTJC.TJDGold.PassingConditions[0].passingType;
                CbCondition2.SelectedIndex = (int)nowTJC.TJDGold.PassingConditions[1].passingType;
                CbCondition3.SelectedIndex = (int)nowTJC.TJDGold.PassingConditions[2].passingType;
                NmRatio1.Value = (decimal)(nowTJC.TJDGold.PassingConditions[0].Ratio * 100);
                NmRatio2.Value = (decimal)(nowTJC.TJDGold.PassingConditions[1].Ratio * 100);
                NmRatio3.Value = (decimal)(nowTJC.TJDGold.PassingConditions[2].Ratio * 100);
            }
        }

        /// <summary>
        /// 条件の有効無効変更
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CbUseCondition_CheckedChanged(object sender, EventArgs e) {
            if (nowTJC.isTJDEnabled) nowTJC.isTJDEnabled = false;
            else nowTJC.isTJDEnabled = true;
        }

        /// <summary>
        /// テストプレイボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtTestPlay_Click(object sender, EventArgs e) {
            var testTJCFInfo = nowTJC.TestPlay();
            TestTJCs.Add(testTJCFInfo);
        }

        /// <summary>
        /// エクスポート
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtCreate_Click(object sender, EventArgs e) {
            var isZip = MessageBox.Show("パックを圧縮しますか？", "圧縮確認", MessageBoxButtons.YesNoCancel);
            if (isZip == DialogResult.Cancel) {
                return;
            }
            var isExportSuccess = TJP.Export(nowTJP, setting, isZip == DialogResult.Yes);
            if (isExportSuccess) {
                MessageBox.Show("エクスポートに成功しました");
            } else {
                MessageBox.Show("エクスポートに失敗しました");
            }
        }

        #region 合格条件変更
        /// <summary>
        /// 合格条件名を変更したら、すべてのコースの合格条件名を変更する
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TbConditionName_TextChanged(object sender, EventArgs e) {
            // 赤合格の名称変更
            if(CbRedOrGold.SelectedIndex == 0) {
                foreach(var tjc in nowTJP.TJCs) {
                    tjc.TJDRed.Name = TbConditionName.Text;
                }
            }
            // 金合格の名称変更
            else if(CbRedOrGold.SelectedIndex == 1) {
                foreach (var tjc in nowTJP.TJCs) {
                    tjc.TJDGold.Name = TbConditionName.Text;
                }
            }
        }

        /// <summary>
        /// 合格条件の閾値を更新します
        /// </summary>
        private void UpdateThresholdByRatio() {
            FlagThresholdEdit = true;
            if ((string)CbCondition1.SelectedItem != "スコア" &&
                (string)CbCondition1.SelectedItem != "連打数" &&
                (string)CbCondition1.SelectedItem != "たたけた数") {
                NmThreshold1.Value = Math.Round(nowTJC.TotalNoteCount() * (NmRatio1.Value / 100), 0);
            }
            if ((string)CbCondition2.SelectedItem != "スコア" &&
                (string)CbCondition2.SelectedItem != "連打数" &&
                (string)CbCondition2.SelectedItem != "たたけた数") {
                NmThreshold2.Value = Math.Round(nowTJC.TotalNoteCount() * (NmRatio2.Value / 100), 0);
            }
            if ((string)CbCondition3.SelectedItem != "スコア" &&
                (string)CbCondition3.SelectedItem != "連打数" &&
                (string)CbCondition3.SelectedItem != "たたけた数") {
                NmThreshold3.Value = Math.Round(nowTJC.TotalNoteCount() * (NmRatio3.Value / 100), 0);
            }
            FlagThresholdEdit = false;
        }

        /// <summary>
        /// 合格条件の割合値を更新します
        /// </summary>
        private void UpdateRatioByThreshold() {
            if (FlagThresholdEdit == true) return;
            var notesCount = nowTJC.TotalNoteCount();
            if (notesCount == 0) return;

            if ((string)CbCondition1.SelectedItem != "スコア" &&
                (string)CbCondition1.SelectedItem != "連打数" &&
                (string)CbCondition1.SelectedItem != "たたけた数") {
                NmRatio1.Value = Math.Round(NmThreshold1.Value / notesCount * 100, 1);
            }
            if ((string)CbCondition2.SelectedItem != "スコア" &&
                (string)CbCondition2.SelectedItem != "連打数" &&
                (string)CbCondition2.SelectedItem != "たたけた数") {
                NmRatio2.Value = Math.Round(NmThreshold2.Value / nowTJC.TotalNoteCount() * 100, 1);
            }
            if ((string)CbCondition3.SelectedItem != "スコア" &&
                (string)CbCondition3.SelectedItem != "連打数" &&
                (string)CbCondition3.SelectedItem != "たたけた数") {
                NmRatio3.Value = Math.Round(NmThreshold3.Value / nowTJC.TotalNoteCount() * 100, 1);
            }
        }

        private void NmThreshold1_ValueChanged(object sender, EventArgs e) {
            UpdateRatioByThreshold();
        }

        private void NmThreshold2_ValueChanged(object sender, EventArgs e) {
            UpdateRatioByThreshold();
        }

        private void NmThreshold3_ValueChanged(object sender, EventArgs e) {
            UpdateRatioByThreshold();
        }

        private void NmRatio1_ValueChanged(object sender, EventArgs e) {
            UpdateThresholdByRatio();
        }

        private void NmRatio2_ValueChanged(object sender, EventArgs e) {
            UpdateThresholdByRatio();
        }

        private void NmRatio3_ValueChanged(object sender, EventArgs e) {
            UpdateThresholdByRatio();
        }
        #endregion

        private void CbTitleInvisible_CheckedChanged(object sender, EventArgs e) {
            nowTJP.TJCs[nowTJCindex].IsTitleHide = CbTitleInvisible.Checked;
        }

        private void CbNumbering_CheckedChanged(object sender, EventArgs e) {
            nowTJP.TJCs[nowTJCindex].IsNumberingEnable = CbNumbering.Checked;
        }

        private void NmNumbering_ValueChanged(object sender, EventArgs e) {
            nowTJP.TJCs[nowTJCindex].Number = (int)NmNumbering.Value;
        }

        private void CbIsTestTJCDelete_CheckedChanged(object sender, EventArgs e) {
            setting.IsTestTJCDelete = CbIsTestTJCDelete.Checked;
        }
    }
}
