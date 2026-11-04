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
                string wOutputLine = CreateOutputLine(wStudentScore, wMaxNameDisplayWidth, wMaxSubjectDisplayWidth, wMaxScoreDisplayWidth);
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
        private string CreateOutputLine(StudentScore vStudentScore, int vNameDisplayWidth, int vSubjectDisplayWidth, int vScoreDisplayWidth) {
            string wName = vStudentScore.Name;
            string wSubject = vStudentScore.Subject;
            int wScore = vStudentScore.Score;

            var wSpacesAfterName = new string(' ', vNameDisplayWidth - CalcDisplayWidth(wName));
            var wSpacesAfterSubject = new string(' ', vSubjectDisplayWidth - CalcDisplayWidth(wSubject));
            var wSpacesBeforeScore = new string(' ', vScoreDisplayWidth - CalcDisplayWidth(wScore.ToString()));

            string[] wEachItem = { wName + wSpacesAfterName, wSubject + wSpacesAfterSubject, wSpacesBeforeScore + wScore };
            return string.Join(" | ", wEachItem);
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
        /// <returns>全教科の平均点（リストが存在しない場合は 0.0）</returns>
        public double CalcOverallAverageScore() {
            if (FStudentScores.Count == 0) {
                return 0.0;
            }
            return FStudentScores.Average(x => x.Score);
        }
    }
}