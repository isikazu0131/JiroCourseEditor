using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using NVorbis;
using UmeboshiLibrary;
using System.Xml.Serialization;

namespace JiroCourseEditor {
    /// <summary>
    /// tjaファイルに関するクラス
    /// </summary>
    public class TJA {

        // 譜面の外部情報
        public string TITLE { get; set; }
        public string SUBTITLE { get; set; }
        public string WAVE { get; set; }
        public double OFFSET { get; set; }
        public double DEMOSTART { get; set; }
        public double BPM { get; set; }
        public double SONGVOL { get; set; }
        public double SEVOL { get; set; }
        public DIFFICULTY COURSE { get; set; }
        public double LEVEL { get; set; }
        public int SCOREMODE { get; set; }
        public BMSCROLL BMSCROLL { get; set; }
        public STYLE STYLE { get; set; }

        /// <summary>
        /// 加算スコア初期値
        /// </summary>
        public double SCOREINIT { get; set; }

            /// <summary>
        /// 加算スコアの加算値
        /// </summary>
        public double SCOREDIFF { get; set; }

        // 譜面の内部情報
        /// <summary>
        /// TJAの完全パス
        /// </summary>
        public string FullName { get; set; }

        /// <summary>
        /// TJAのファイル名+拡張子
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// ノーツ情報
        /// </summary>
        //public List<Note> Notes { get; set; }

        /// <summary>
        /// 小節情報
        /// </summary>
        public List<Measure> Measures { get; set; }

        /// <summary>
        /// ノーツ数
        /// </summary>
        public int NotesCount { get; set; }

        /// <summary>
        /// ノーツ数（P1）
        /// </summary>
        public int NotesP1 { get; set; }

        /// <summary>
        /// ノーツ数（P2）
        /// </summary>
        public int NotesP2 { get; set; }

        /// <summary>
        /// 最高BPM
        /// </summary>
        public double MaxBPM { get; set; }

        /// <summary>
        /// 最低BPM
        /// </summary>
        public double MinBPM { get; set; }

        /// <summary>
        /// 音源再生時間
        /// </summary>
        public double MusicPlayTime { get; set; }

        // コンストラクタ
        public TJA() {

        }

        /// <summary>
        /// 指定したフォルダ内にあるTJAをリストにまとめて返す
        /// </summary>
        /// <param name="directoryInfo"></param>
        /// <returns></returns>
        static public List<TJA> GetTJAs(DirectoryInfo directoryInfo) {
            var tja_infos = directoryInfo.GetFiles("*.tja", SearchOption.AllDirectories);
            List<TJA> TJAs = new List<TJA>();

            List<Task> tasks = new List<Task>();
            foreach (var tja in tja_infos) {
                var task = Task.Run(() => {
                    TJAs.Add(new TJA(tja));
                });
                tasks.Add(task);
            }

            Task.WaitAll(tasks.ToArray());
            TJAs = TJAs.OrderBy(x => x.FullName).ToList();
            return TJAs;
        }

        /// <summary>
        /// 指定したtjaファイルの情報を格納する
        /// </summary>
        /// <param name="tjaFilePath"></param>
        public static TJA SetTJAbyPath(string tjaFilePath) {
            FileInfo tjaFInfo = new FileInfo(tjaFilePath);
            return new TJA(tjaFInfo);
        }

        /// <summary>
        /// 指定したTJAファイルの情報を格納する
        /// </summary>
        /// <param name="TjaFileInfo"></param>
        public TJA(FileInfo TjaFileInfo) {
            try {
                FullName = TjaFileInfo.FullName;
                FileName = TjaFileInfo.Name;
                //Notes = new List<Note>();
                //System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance); // memo: Shift-JISを扱うためのおまじない

                var Contents = File.ReadAllLines(TjaFileInfo.FullName, Encoding.GetEncoding("Shift_JIS"));
                this.STYLE = STYLE.SP;
                this.BMSCROLL = BMSCROLL.NONE;
                this.COURSE = DIFFICULTY.ONI;

                // 抽出した文字列を一時格納
                string extracted_data;
                foreach (var line in Contents) {

                    string processedline = DeleteComment(line).Trim(); // 余計なコメントを削除します

                    if (processedline.StartsWith("TITLE:")) {
                        extracted_data = UmeboshiString.CutToEnd(processedline, "TITLE:");
                        TITLE = extracted_data;
                    }
                    if (processedline.StartsWith("SUBTITLE:")) {
                        extracted_data = UmeboshiString.CutToEnd(processedline, "SUBTITLE:");
                        SUBTITLE = extracted_data;
                    }
                    if (processedline.StartsWith("WAVE:")) {
                        extracted_data = UmeboshiString.CutToEnd(processedline, "WAVE:");
                        WAVE = extracted_data;
                    }
                    if (processedline.StartsWith("OFFSET:")) {
                        extracted_data = UmeboshiString.CutToEnd(processedline, "OFFSET:");
                        extracted_data = DataToNumString(extracted_data);
                        if (double.TryParse(extracted_data, out double extracted_data_converted) == false) {
                            OFFSET = 0;
                            continue;
                        }
                        OFFSET = extracted_data_converted;
                    }
                    if (processedline.StartsWith("DEMOSTART:")) {
                        extracted_data = UmeboshiString.CutToEnd(processedline, "DEMOSTART:");
                        extracted_data = DataToNumString(extracted_data);
                        if (String.IsNullOrEmpty(extracted_data) || double.TryParse(extracted_data, out double extracted_data_converted) == false) {
                            DEMOSTART = 0;
                            continue;
                        }
                        DEMOSTART = extracted_data_converted;
                    }
                    if (processedline.StartsWith("BPM:")) {
                        extracted_data = UmeboshiString.CutToEnd(processedline, "BPM:").Trim();
                        extracted_data = System.Text.RegularExpressions.Regex.Matches(extracted_data, @"[0-9]+\.?[0-9]*")[0].Value;
                        BPM = double.Parse(extracted_data);
                        MinBPM = BPM;
                        MaxBPM = BPM;
                    }
                    if (processedline.StartsWith("SONGVOL:")) {
                        extracted_data = UmeboshiString.CutToEnd(processedline, "SONGVOL:");
                        extracted_data = DataToNumString(extracted_data);
                        if (String.IsNullOrEmpty(extracted_data) || double.TryParse(extracted_data, out double extracted_data_converted) == false) {
                            SONGVOL = 100;
                            continue;
                        }
                        SONGVOL = double.Parse(extracted_data);
                    }
                    if (processedline.StartsWith("SEVOL:")) {
                        extracted_data = UmeboshiString.CutToEnd(processedline, "SEVOL:");
                        extracted_data = DataToNumString(extracted_data);
                        if (String.IsNullOrEmpty(extracted_data) || double.TryParse(extracted_data, out double extracted_data_converted) == false) {
                            SEVOL = 100;
                            continue;
                        }
                        SEVOL = double.Parse(UmeboshiString.CutToEnd(extracted_data, "SEVOL:"));
                    }
                    if (processedline.StartsWith("COURSE:")) {
                        extracted_data = UmeboshiString.CutToEnd(processedline, "COURSE:");
                        COURSE = GetCourse(extracted_data);
                    }
                    if (processedline.StartsWith("#BPMCHANGE")) {
                        extracted_data = UmeboshiString.CutToEnd(processedline, "#BPMCHANGE");
                        if (String.IsNullOrEmpty(extracted_data) || double.TryParse(extracted_data, out double ChangedBPM) == false) {
                            continue;
                        }
                        if (MaxBPM < ChangedBPM) MaxBPM = ChangedBPM;
                        if (MinBPM > ChangedBPM) MinBPM = ChangedBPM;
                    }
                    if (processedline.StartsWith("LEVEL:")) {
                        extracted_data = UmeboshiString.CutToEnd(processedline, "LEVEL:");
                        extracted_data = DataToNumString(extracted_data);
                        if (String.IsNullOrEmpty(extracted_data) || double.TryParse(extracted_data, out double extracted_data_converted) == false) {
                            LEVEL = 0;
                            continue;
                        }
                        LEVEL = extracted_data_converted;
                    }
                    if (processedline.StartsWith("STYLE:")) {
                        extracted_data = UmeboshiString.CutToEnd(processedline, "STYLE:");
                        extracted_data = DataToNumString(extracted_data);
                        if (String.IsNullOrEmpty(extracted_data) || int.TryParse(extracted_data, out int extracted_data_converted) == false) {
                            STYLE = STYLE.SP;
                            continue;
                        }
                        switch (extracted_data_converted) {
                            case 1: 
                                STYLE = STYLE.SP;
                                break;
                            case 2: 
                                STYLE = STYLE.DP;
                                break;
                            default:
                                STYLE = STYLE.SP;
                                break;
                        };
                    }
                    if (processedline.StartsWith("#BMSCROLL")) {
                        BMSCROLL = BMSCROLL.BMSCROLL;
                    }
                    if (processedline.StartsWith("#HBSCROLL")) {
                        BMSCROLL = BMSCROLL.HBSCROLL;
                    }
                }

                if (STYLE == STYLE.SP) {
                    NotesCount = GetNotes(Contents, "");
                }
                else if(STYLE == STYLE.DP) {
                    NotesP1 = GetNotes(Contents, "P1");
                    NotesP2 = GetNotes(Contents, "P2");
                    NotesCount = NotesP1 + NotesP2;
                }

                MusicPlayTime = GetOggFullTime(Path.Combine(TjaFileInfo.DirectoryName, WAVE));

                if (SUBTITLE == null) SUBTITLE = "";
            }
            catch (Exception ex) {
                // ログを吐く
                // MessageBox.Show(ex.Message);
            }

        }

        static public string CourseToString(DIFFICULTY diff) {
            switch (diff) {
                case DIFFICULTY.EASY:
                    return "かんたん";
                case DIFFICULTY.NORMAL:
                    return "ふつう";
                case DIFFICULTY.HARD:
                    return "むずかしい";
                case DIFFICULTY.ONI:
                    return "おに";
                case DIFFICULTY.EDIT:
                    return "edit";
                default:
                    return "そのほか";
            }
        }

        /// <summary>
        /// 数値情報に変換します
        /// </summary>
        private string DataToNumString(string data) {
            if (System.Text.RegularExpressions.Regex.Matches(data, @"[0-9]+\.?[0-9]*").Count == 0) {
                return "";
            }
            return System.Text.RegularExpressions.Regex.Matches(data, @"[0-9]+\.?[0-9]*")[0].Value;
        }

        private DIFFICULTY GetCourse(string coursetxt) {
            if (int.TryParse(coursetxt, out var coursevalue)) {
                switch (coursevalue) {
                    case 0: return DIFFICULTY.EASY;
                    case 1: return DIFFICULTY.NORMAL;
                    case 2: return DIFFICULTY.HARD;
                    case 3: return DIFFICULTY.ONI;
                    case 4: return DIFFICULTY.EDIT;
                    default: return DIFFICULTY.ONI;
                }
            } else {
                switch (coursetxt) {
                    case "Easy": return DIFFICULTY.EASY;
                    case "Normal": return DIFFICULTY.NORMAL;
                    case "Hard": return DIFFICULTY.HARD;
                    case "Oni": return DIFFICULTY.ONI;
                    case "Edit": return DIFFICULTY.EDIT;
                    default: return DIFFICULTY.ONI;
                }
            }
        }

        /// <summary>
        /// ノーツ数をカウント、音符の取得をします
        /// </summary>
        /// <param name="Contains"></param>
        /// <returns></returns>
        private int GetNotes(string[] Contents, string PlaySide) {
            int count = 0; // 現在の行数取得
            int combo = 0; // コンボ数
            bool isTJA = false;
            bool isDP = false;
            string DeleteCommentContent; // コメントを削除した行
            while (count < Contents.Length) {

                if (Contents[count].StartsWith("#START")) {

                    // DPかつ指定した側の譜面じゃない場合スキップ
                    if(PlaySide != "" && Contents[count].Contains(PlaySide) != true) {
                        count++;
                        continue;
                    }
                    count++;
                    isTJA = true;
                    break;
                }
                count++;
            }
            if (isTJA == false) return 0;
            for (int i = count; i < Contents.Length; i++) {
                DeleteCommentContent = DeleteComment(Contents[i]);
                if (DeleteCommentContent.StartsWith("#MEASURE") ||
                    DeleteCommentContent.StartsWith("#BPMCHANGE") ||
                    DeleteCommentContent.StartsWith("#SCROLL") ||
                    DeleteCommentContent.StartsWith("#DELAY") ||
                    DeleteCommentContent.StartsWith("//")) continue;
                // 「1,2,3,4」の個数を計測
                for (int j = 0; j < Contents[i].Length; j++) {
                    if (Contents[i][j] == '1' || Contents[i][j] == '2' || Contents[i][j] == '3' || Contents[i][j] == '4') {
                        // Notes.Add(new Note(int.Parse(Contents[i][j].ToString()), combo));
                        combo++;
                    }
                }

                if (Contents[i].StartsWith("#END")) break;
            }

            return combo;
        }

        /// <summary>
        /// 余計なコメントを削除します
        /// </summary>
        /// <param name="line">読み込まれた行</param>
        /// <returns></returns>
        private string DeleteComment(string line) {
            if (line.Contains("//") == false) return line;
            else if (line.StartsWith("//")) return line;
            try {
                return UmeboshiString.CutToStart(line, "//");
            }
            catch {
                return "";
            }

        }

        /// <summary>
        /// 音源ファイルの長さを取得します
        /// </summary>
        /// <param name="oggPath"></param>
        /// <returns></returns>
        private double GetOggFullTime(string oggPath) {
            if (!File.Exists(oggPath)) return 0;
            VorbisReader reader = new VorbisReader(oggPath);
            TimeSpan timeSpan = reader.TotalTime;
            reader.Dispose();
            return timeSpan.TotalSeconds;
        }

    }

    public class Note {
        public NoteKind noteKind;
        public Fingering fingering;
        public double timing;

        public Note(int kind, int count) {
            noteKind = (NoteKind)kind;
            if (count % 2 == 0) {
                fingering = Fingering.Right;
            } else {
                fingering = Fingering.Left;
            }
        }

        public Note(int kind) {
            noteKind = (NoteKind)kind;
        }

        public enum Fingering {
            Left,
            Right
        }

        public enum NoteKind {
            None,
            Dong,
            Ka,
            DongL,
            KaL,
            Roll,
            RollL,
            Balloon,
            RollEnd,
            Poteto
        }
    }

    /// <summary>
    /// 各小節の内部情報
    /// </summary>
    public class Measure {

        /// <summary>
        /// 現在のBPM
        /// </summary>
        public double BPM;

        /// <summary>
        /// 拍子の分母部分
        /// </summary>
        public double Measure_den;

        /// <summary>
        /// 拍子の分子部分
        /// </summary>
        public double Measure_mol;

        /// <summary>
        /// 小節内のノーツ情報
        /// </summary>
        public List<Note> Notes;

        /// <summary>
        /// 1小節の1行を参照
        /// </summary>
        /// <param name="Content"></param>
        public Measure(string Content, double den, double mol) {
            Notes = new List<Note>();
            Measure_den = den;
            Measure_mol = mol;
            foreach (var note in Content) {
                Notes.Add(new Note(int.Parse(note.ToString())));
            }
        }

        //    /// <summary>
        //    /// 1小節あたりの時間を計算
        //    /// </summary>
        //    /// <param name="MeasureString"></param>
        //    /// <returns></returns>
        //    public double CalcMeasureTime(List<string> MeasureString, double BPM, double den, double mol) {
        //        // 現在のノーツ数
        //        int notecount = 0;

        //        // 経過時間
        //        double elapsed = 0;

        //        foreach(var line in MeasureString) {
        //            if (line.StartsWith("#MEASURE")) {
        //                continue;
        //            }
        //            if (line.StartsWith("#BPMCHANGE")) {
        //                if(MeasureRow.Length != 0) {

        //                }
        //                NowBPM = double.Parse(UmeboshiString.CutToEnd(line, "#BPMCHNGE"));
        //                continue;

        //            }
        //            if (line.StartsWith("#DELAY")) {
        //                elapsed += double.Parse(UmeboshiString.CutToEnd(line, "#DELAY"));
        //                continue;

        //            }
        //            if (line.StartsWith("#SCROLL")) {
        //                continue;
        //            }

        //            if (line.EndsWith(",")) {
        //                // 1行で書かれた1小節
        //                if(notecount == 0) {
        //                    elapsed = 60 / BPM * 4 * (mol / den);
        //                }
        //            }
        //        }

        //        return elapsed
        //    }

        //}

    }

    public enum DIFFICULTY {
        EASY,
        NORMAL,
        HARD,
        ONI,
        EDIT
    }

    public enum BMSCROLL {
        NONE,
        BMSCROLL,
        HBSCROLL
    }

    public enum STYLE {
        SP,
        DP
    }
}
