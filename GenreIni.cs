using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JiroCourseEditor {
    /// <summary>
    /// Genre.ini関連クラス
    /// </summary>
    public class GenreIni {
        /// <summary>
        /// Genre.iniを出力します
        /// </summary>
        /// <param name="GenreName"></param>
        /// <param name="GanreColor"></param>
        /// <param name="FontColor"></param>
        /// <returns></returns>
        public static bool Write(string outputFolder, string GenreName, string GanreColor, string FontColor) {
            try {
                using (StreamWriter sw = new StreamWriter(Path.Combine(outputFolder, Constants.FileName.GenreIniFile), false, Encoding.GetEncoding("Shift_jis"))) {
                    // 出力する文字列
                    List<string> lines = new List<string>();
                    lines.Add("[Genre]");
                    lines.Add($"GenreName={GenreName}");
                    lines.Add($"GenreColor={GanreColor}");
                    lines.Add($"FontColor={FontColor}");
                    foreach (string line in lines) { sw.WriteLine(line); }
                }
                return true;
            }
            catch (Exception ex) { 
                return false;
            }
        }
    }
}
