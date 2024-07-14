using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace JiroPackEditor {
    /// <summary>
    /// 共通で使うメソッド群
    /// </summary>
    public static class GrobalMethod {
        public static string CheckName(string title) {
            // 禁止文字
            char[] invalidChars = { '\\', '/', ':', '*', '?', '"', '<', '>', '|' };
            foreach (char c in invalidChars) {
                if (title.Contains(c)) return c.ToString();
            }
            return "";
        }

        public static string CutInvalidChar(string title) {
            // 禁止文字の正規表現パターン
            string invalidCharsPattern = @"[\\/:*?""<>|]";

            // 禁止文字を空文字に置換
            return Regex.Replace(title, invalidCharsPattern, "");
        }
    }
}
