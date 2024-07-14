using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JiroPackEditor {
    /// <summary>
    /// 定数を扱う総合クラス
    /// </summary>
    public static class Constants {

        /// <summary>
        /// 当アプリの基本情報の定数クラス
        /// </summary>
        public static class AppInfo {
            /// <summary>
            /// アプリケーション名
            /// </summary>
            public const string Name = "Jiro Pack Editor";

            /// <summary>
            /// バージョン
            /// </summary>
            public const string Version = "V2.0.0";

            public const string GitHubLink = "https://github.com/isikazu0131/JiroCourseEditor";
        }

        /// <summary>
        /// 当アプリで使用されるファイル関連の定数クラス
        /// </summary>
        public static class FileName {
            /// <summary>
            /// 設定フォルダ名
            /// </summary>
            public const string SettingFolder = @"Setting";

            /// <summary>
            /// 設定ファイル名
            /// </summary>
            public const string SettingMainFile = @"Setting\SettingMain.json";

            /// <summary>
            /// Genre.iniファイル名
            /// </summary>
            public const string GenreIniFile = @"Genre.ini";

        }

        /// <summary>
        /// パックで出力されるフォルダ名
        /// </summary>
        public static class DirectoryName {
            /// <summary>
            /// 段位フォルダ名（他の名前にしたかったら適宜エクスポート後に変えてもらう）
            /// </summary>
            public const string Course = "段位";

            /// <summary>
            /// 課題曲
            /// </summary>
            public const string Songs = "課題曲";

            /// <summary>
            /// tjd
            /// </summary>
            public const string tjd = "tjd";
        }

        /// <summary>
        /// TJPのデフォルト値
        /// </summary>
        public static class TJPDefault {
            public const string Name = "新しいパック";

            public static Color PackBackColor = Color.Black;
            public static Color PackForeColor = Color.White;
            public static Color CourseBackColor = Color.Black;
            public static Color CourseForeColor = Color.Red;
            public static Color SongBackColor = Color.Black;
            public static Color SongForeColor = Color.DeepSkyBlue;
        }

        /// <summary>
        /// 拡張子関連の定数クラス
        /// </summary>
        public static class Extention {
            /// <summary>
            /// パックファイルの拡張子
            /// </summary>
            public const string TJP = ".tjp";

            /// <summary>
            /// 次郎の譜面ファイル拡張子（小文字）
            /// </summary>
            public const string tja = ".tja";

            /// <summary>
            /// 次郎の譜面ファイル拡張子（大文字）
            /// </summary>
            public const string TJA = ".TJA";

            /// <summary>
            /// コースファイルの拡張子
            /// </summary>
            public const string TJC = ".tjc";

            /// <summary>
            /// 段位さん次郎用拡張子
            /// </summary>
            public const string TJD = ".tjd";
        }

    }
}
