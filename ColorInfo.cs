using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JiroCourseEditor {
    public class ColorInfo {

        public static string GetColorCode(Color color) {
            return $"#{color.R:X2}{color.G:X2}{color.B:X2}";
        }

        public static Color GetColor(string colorCode) { 
            return ColorTranslator.FromHtml(colorCode);
        }
    }
}
