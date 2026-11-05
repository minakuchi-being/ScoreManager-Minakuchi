using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace ScoreManager {
    /// <summary>
    /// 成績に関する処理（画面表示や平均点計算など）を担当するクラス
    /// </summary>
    internal class ScoreManager {
        /// <summary>
        /// 名前のバリデーション（ひらがな / カタカナ / 漢字 / 英字 のみ可）
        /// </summary>
        private const string C_RegexPatternOfName = @"^[\p{IsHiragana}\p{IsKatakana}\p{IsCJKUnifiedIdeographs}a-zA-Z]+$";

        /// <summary>
        /// 科目のバリデーション（ひらがな / カタカナ / 漢字 のみ可）
        /// </summary>
        private const string C_RegexPatternOfSubject = @"^[\p{IsHiragana}\p{IsKatakana}\p{IsCJKUnifiedIdeographs}]+$";

        /// <summary>
        /// 点数のバリデーション（0～100 の整数値 のみ可）
        /// </summary>
        private const string C_RegexPatternOfScoreText = @"^(100|[1-9]?[0-9])$";

        /// <summary>
        /// Shift-JIS のエンコード
        /// </summary>
        private static readonly Encoding C_EncodingShiftJIS = Encoding.GetEncoding("shift_jis");

        /// <summary>
        /// 不正でない生徒のデータのクラスを保持するリスト
        /// </summary>
        private List<StudentScore> FStudentScores = new List<StudentScore>();

        /// <summary>
        /// CSVファイルを読み込み、不正行を除いてリストに格納するメソッド
        /// </summary>
        /// <param name="vFilePath">読み込みたいCSVファイルのパス</param>
        /// <param name="vStatus">メソッド終了時の状態</param>
        /// <returns>今後の処理継続の可否</returns>
        public bool TryLoadCSV(string vFilePath, out LoadCSVStatusEnum vStatus) {
            if (!File.Exists(vFilePath)) {
                vStatus = LoadCSVStatusEnum.FileNotFound;
                return false;
            }
            using (var wFile = new StreamReader(vFilePath, Encoding.UTF8)) {
                while (!wFile.EndOfStream) {
                    string wLine = wFile.ReadLine();

                    if (string.IsNullOrWhiteSpace(wLine) || wLine.TrimStart().StartsWith("#")) {
                        continue;
                    }
                    AddValidatedScoreToList(wLine);
                }
            }
            int wValidScoreCount = FStudentScores.Count;
            if (wValidScoreCount == 0) {
                vStatus = LoadCSVStatusEnum.NoValidScore;
                return false;
            }
            Console.WriteLine($"{wValidScoreCount} 件のデータを読み込みました。");
            vStatus = LoadCSVStatusEnum.Success;
            return true;
        }

        /// <summary>
        /// バリデーションに沿った成績のみリストに格納するメソッド
        /// </summary>
        /// <param name="vLine">CSVの1行</param>
        private void AddValidatedScoreToList(string vLine) {
            string[] wItems = vLine.Split(',');
            if (wItems.Length != 3 || wItems.Any(x => string.IsNullOrWhiteSpace(x))) {
                Console.WriteLine($"不正な行（項目数不正）: {vLine}");
                return;
            }
            string wName = wItems[0].Trim();
            if (!Regex.IsMatch(wName, C_RegexPatternOfName)) {
                Console.WriteLine($"名前が不正: {wName}");
                return;
            }
            string wSubject = wItems[1].Trim();
            if (!Regex.IsMatch(wSubject, C_RegexPatternOfSubject)) {
                Console.WriteLine($"科目が不正: {wSubject}");
                return;
            }
            string wScoreText = wItems[2].Trim();
            if (!Regex.IsMatch(wScoreText, C_RegexPatternOfScoreText)) {
                Console.WriteLine($"成績が不正: {wScoreText}");
                return;
            }
            var wStudentScore = new StudentScore(wName, wSubject, int.Parse(wScoreText));
            FStudentScores.Add(wStudentScore);
        }

        /// <summary>
        /// 成績一覧を表示するメソッド
        /// </summary>
        public void DisplayAllScores() {
            if (FStudentScores.Count == 0) {
                Console.WriteLine("表示する成績がありません。");
                return;
            }
            int wMaxNameDisplayWidth = FStudentScores.Max(x => CalcDisplayWidth(x.Name));
            int wMaxSubjectDisplayWidth = FStudentScores.Max(x => CalcDisplayWidth(x.Subject));
            int wMaxScoreDisplayWidth = FStudentScores.Max(x => CalcDisplayWidth(x.Score.ToString()));

            foreach (var wStudentScore in FStudentScores) {
                string wOutputLine = CreateScoreOutputLine(wStudentScore, wMaxNameDisplayWidth, wMaxSubjectDisplayWidth, wMaxScoreDisplayWidth);
                Console.WriteLine(wOutputLine);
            }
        }

        /// <summary>
        /// 成績1行の出力文を作成するメソッド
        /// </summary>
        /// <param name="vStudentScore">1つの成績データ</param>
        /// <param name="vNameDisplayWidth">名前欄に割り当てたい画面表示幅</param>
        /// <param name="vSubjectDisplayWidth">科目欄に割り当てたい画面表示幅</param>
        /// <param name="vScoreDisplayWidth">点数欄に割り当てたい画面表示幅</param>
        /// <returns>成績1行の出力文</returns>
        private string CreateScoreOutputLine(StudentScore vStudentScore, int vNameDisplayWidth, int vSubjectDisplayWidth, int vScoreDisplayWidth) {
            string[] wEachItem = { PadRightByDisplayWidth(vStudentScore.Name, vNameDisplayWidth),
                                   PadRightByDisplayWidth(vStudentScore.Subject, vSubjectDisplayWidth),
                                   PadLeftByDisplayWidth(vStudentScore.Score.ToString(), vScoreDisplayWidth) };
            return string.Join(" | ", wEachItem);
        }

        /// <summary>
        /// 指定された画面表示幅になるように半角スペースを追加して、文字列を左詰めするメソッド
        /// </summary>
        /// <param name="vText">左詰めしたい文字列</param>
        /// <param name="vTextDisplayWidth">画面表示幅</param>
        /// <returns>空白で左詰めされた文字列</returns>
        private string PadRightByDisplayWidth(string vText, int vTextDisplayWidth) {
            return vText + new string(' ', vTextDisplayWidth - CalcDisplayWidth(vText));
        }

        /// <summary>
        /// 指定された画面表示幅になるように半角スペースを追加して、文字列を右詰めするメソッド
        /// </summary>
        /// <param name="vText">右詰めしたい文字列</param>
        /// <param name="vTextDisplayWidth">画面表示幅</param>
        /// <returns>空白で右詰めされた文字列</returns>
        private string PadLeftByDisplayWidth(string vText, int vTextDisplayWidth) {
            return new string(' ', vTextDisplayWidth - CalcDisplayWidth(vText)) + vText;
        }

        /// <summary>
        /// コンソール画面で表示した時、半角何文字分の幅が必要かを計算するメソッド
        /// </summary>
        /// <param name="vText">調べたい文字列</param>
        /// <returns>画面表示時に必要な幅（半角換算）</returns>
        private int CalcDisplayWidth(string vText) {
            return C_EncodingShiftJIS.GetByteCount(vText);
        }

        /// <summary>
        /// 全教科の平均点を計算するメソッド
        /// </summary>
        /// <returns>全教科の平均点（成績が1件も存在しない場合は 0.0）</returns>
        public double CalcOverallAverageScore() {
            if (FStudentScores.Count == 0) {
                return 0.0;
            }
            return FStudentScores.Average(x => x.Score);
        }

        /// <summary>
        /// 各教科の平均点を計算するメソッド
        /// </summary>
        /// <returns>各教科の平均点の辞書（成績が1件も存在しない場合は空の辞書）</returns>
        public Dictionary<string, double> CalcSubjectAverageScores() {
            if (FStudentScores.Count == 0) {
                return new Dictionary<string, double>();
            }
            var wSubjectAverageScores = FStudentScores.GroupBy(x => x.Subject).ToDictionary(x => x.Key, x => x.Average(y => y.Score));
            return wSubjectAverageScores;
        }

        /// <summary>
        /// 各教科の平均点を表示するメソッド
        /// </summary>
        /// <param name="vSubjectAverageScores">科目名と平均点の辞書</param>
        public void DisplaySubjectAverageScores(Dictionary<string, double> vSubjectAverageScores) {
            if (vSubjectAverageScores.Count == 0) {
                Console.WriteLine("表示する科目別平均点がありません。");
                return;
            }
            int wMaxSubjectDisplayWidth = vSubjectAverageScores.Max(x => CalcDisplayWidth(x.Key));
            int wMaxScoreDisplayWidth = vSubjectAverageScores.Max(x => CalcDisplayWidth(x.Value.ToString("F2")));

            foreach (KeyValuePair<string, double> wSubjectAverageScore in vSubjectAverageScores) {
                string wOutputLine = CreateAverageOutputLine(wSubjectAverageScore.Key, wSubjectAverageScore.Value, wMaxSubjectDisplayWidth, wMaxScoreDisplayWidth);
                Console.WriteLine(wOutputLine);
            }
        }

        /// <summary>
        /// 平均点1行の出力文を作成する
        /// </summary>
        /// <param name="vSubject">科目名</param>
        /// <param name="vScore">平均点</param>
        /// <param name="vSubjectDisplayWidth">科目名に割り当てたい画面表示幅</param>
        /// <param name="vScoreDisplayWidth">平均点欄に割り当てたい画面表示幅</param>
        /// <returns>平均点1行の出力文</returns>
        private string CreateAverageOutputLine(string vSubject, double vScore, int vSubjectDisplayWidth, int vScoreDisplayWidth) {
            string[] wEachItem = { PadRightByDisplayWidth(vSubject, vSubjectDisplayWidth),
                                   PadLeftByDisplayWidth(vScore.ToString("F2"), vScoreDisplayWidth) };
            return string.Join(": ", wEachItem);
        }
    }
}