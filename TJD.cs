using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace JiroPackEditor {
    /// <summary>
    /// 段位さん次郎用設定ファイルフォーマット
    /// </summary>
    public class TJD {
        /// <summary>
        /// 合格条件リスト(最大要素数 3)
        /// </summary>
        public List<PassingCondition> PassingConditions { get; set; }

        /// <summary>
        /// 合格条件名称
        /// </summary>
        public string Name { get; set; }

        public TJD(string name) {
            Name = name;
            //初期値
            PassingConditions = new List<PassingCondition>() {
                new PassingCondition(PassingType.None, 0, 0),
                new PassingCondition(PassingType.None, 0, 0),
                new PassingCondition(PassingType.None, 0, 0)
            };
        }
        
        /// <summary>
        /// 出力する（赤・金合格を分ける）
        /// </summary>
        /// <param name="tjd"></param>
        public void Write(string outputFolder, string courseName) {
            try {
                // 合格条件「なし」のみの条件の場合、メッセージを出力してtjdは作成しない
                if (!this.PassingConditions.Any(x => x.passingType != PassingType.None)) {

                    Setting setting = Setting.Read();
                    if (setting.IsViewTJDNoneMsg == false) {
                        MessageBoxOnce messageBoxOnce = new MessageBoxOnce();
                        messageBoxOnce.Text = "確認";
                        messageBoxOnce.msg = $"{courseName}の{this.Name}条件がすべて「なし」に設定されているため、tjdを出力しません";
                        messageBoxOnce.ShowDialog();
                        // 現在の設定を上書き
                        if (setting.IsViewTJDNoneMsg != messageBoxOnce.FlgNextView) {
                            setting.IsViewTJDNoneMsg = messageBoxOnce.FlgNextView;
                            Setting.Write(setting);
                        }
                    }
                    return;
                }
                string outputTJDPath = Path.Combine(outputFolder, $"{courseName}_{Name}{Constants.Extention.TJD}");
                // まずは条件の種類を書く
                foreach (PassingCondition condition in PassingConditions) {
                    File.AppendAllText(outputTJDPath, ((int)condition.passingType).ToString() + Environment.NewLine);
                }
                // 条件の閾値を書く
                foreach (PassingCondition condition in PassingConditions) {
                    File.AppendAllText(outputTJDPath, condition.Threshold.ToString() + Environment.NewLine);
                }
            }
            catch (Exception ex){
                MessageBox.Show("TJDの出力に失敗しました。");
            }
        }

        /// <summary>
        /// MJE向けTJD設定
        /// </summary>
        public static TJD CreateTJDforMJE(bool isSP, bool isRed, int level) {
            string name = isRed ? "2次合格" : "3次合格";
            TJD tjd = new TJD(name);
            tjd.PassingConditions = new List<PassingCondition>();
            if (isSP) {
                // 条件① なし
                PassingCondition conditionNone = new PassingCondition();
                conditionNone.passingType = PassingType.None;
                conditionNone.Threshold = 0;
                // 条件② 可の数
                PassingCondition conditionGoodCount = new PassingCondition();
                conditionGoodCount.passingType = PassingType.GoodCount;
                // 条件③ 不可の数
                PassingCondition conditionBadCount = new PassingCondition();
                conditionBadCount.passingType = PassingType.BadCount;
                // 赤合格（二次合格の場合）
                if (isRed) {
                    if (level >= 1 && level <= 5) {
                        conditionGoodCount.Ratio = 0.2;
                        conditionBadCount.Ratio = 0.05;
                    } else if (level >= 6 && level <= 10) {
                        conditionGoodCount.Ratio = 0.15;
                        conditionBadCount.Ratio = 0.03;
                    } else if (level >= 11) {
                        conditionGoodCount.Ratio = 0.1;
                        conditionBadCount.Ratio = 0.015;
                    }
                }
                // 金合格（三次合格の場合）
                else {
                    if (level >= 1 && level <= 5) {
                        conditionGoodCount.Ratio = 0.05;
                        conditionBadCount.Ratio = 0.015;
                    } else if (level >= 6 && level <= 10) {
                        conditionGoodCount.Ratio = 0.03;
                        conditionBadCount.Ratio = 0.01;
                    } else if (level >= 11) {
                        conditionGoodCount.Ratio = 0.02;
                        conditionBadCount.Ratio = 0.005;
                    }
                }
                tjd.PassingConditions.Add(conditionNone);
                tjd.PassingConditions.Add(conditionGoodCount);
                tjd.PassingConditions.Add(conditionBadCount);
                return tjd;
            } 
            // DPの場合TJDなし = 条件なし
            else {
                PassingCondition conditionNone = new PassingCondition();
                conditionNone.passingType = PassingType.None;
                conditionNone.Threshold = 0;
                // 3つすべて条件なしで設定
                tjd.PassingConditions.Add(conditionNone);
                tjd.PassingConditions.Add(conditionNone);
                tjd.PassingConditions.Add(conditionNone);
                return tjd;
            }
        }
    }

    /// <summary>
    /// 合格条件クラス
    /// </summary>
    public class PassingCondition {
        /// <summary>
        /// 合格条件種類
        /// </summary>
        public PassingType passingType { get; set; }
        /// <summary>
        /// 閾値
        /// </summary>
        public int Threshold { get; set; }
        /// <summary>
        /// 割合（ノーツ数のうち何%まで許容するかなど）
        /// （100%を1とした0～1までの値）
        /// </summary>
        public double Ratio { get; set; }
        
        public PassingCondition() {

        }

        public PassingCondition(PassingType passingType, int threshold, double ratio) {
            this.passingType = passingType;
            Threshold = threshold;
            Ratio = ratio;
        }
    }

    /// <summary>
    /// 合格条件種類
    /// </summary>
    public enum PassingType {
        /// <summary>
        /// なし
        /// </summary>
        None = 7,
        /// <summary>
        /// スコア
        /// </summary>
        Score = 0,
        /// <summary>
        /// 良の数
        /// </summary>
        GreatCount = 1,
        /// <summary>
        /// 可の数
        /// </summary>
        GoodCount = 2,
        /// <summary>
        /// 不可の数
        /// </summary>
        BadCount = 3,
        /// <summary>
        /// 連打数
        /// </summary>
        RoleCount = 4,
        /// <summary>
        /// コンボ数
        /// </summary>
        MaxCombo = 5,
        /// <summary>
        /// 叩けた数
        /// </summary>
        HitCount = 6,
    }
}
