using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace ScoreManager {
    /// <summary>
    /// 生徒のデータを扱う処理を担当するクラス
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
        /// 不正でない生徒のデータのクラスを保持するリスト
        /// </summary>
        private List<StudentScore> FStudentScores = new List<StudentScore>();

        /// <summary>
        /// CSVファイルを読み込み、不正行を除いてリストに格納するメソッド
        /// </summary>
        /// <param name="vFilePath">読み込みたいCSVファイルのパス</param>
        /// <returns>リストに格納された件数（ファイルが存在しなかった場合は-1）</returns>
        public int LoadCSV(string vFilePath) {

            if (!File.Exists(vFilePath)) {
                Console.WriteLine($"{vFilePath} は存在していません。");
                return -1;
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
            int wValidatedScoreCount = FStudentScores.Count;
            Console.WriteLine($"{wValidatedScoreCount} 件のデータを読み込みました。");
            return wValidatedScoreCount;
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

            int wMaxNameDisplayWidth = FStudentScores.Max(x => CalcDisplayWidth(x.Name));
            int wMaxSubjectDisplayWidth = FStudentScores.Max(x => CalcDisplayWidth(x.Subject));
            int wMaxScoreDisplayWidth = FStudentScores.Max(x => CalcDisplayWidth(x.Score.ToString()));

            foreach (var wStudentScore in FStudentScores) {
                string wFormat = CreateOutputFormat(wStudentScore, wMaxNameDisplayWidth, wMaxSubjectDisplayWidth, wMaxScoreDisplayWidth);
                Console.WriteLine(wFormat, wStudentScore.Name, wStudentScore.Subject, wStudentScore.Score);
            }
        }

        /// <summary>
        /// 成績1行の出力書式を作成するメソッド
        /// </summary>
        /// <param name="vStudentScore">1つの成績データ</param>
        /// <param name="vNameDisplayWidth">名前欄に割り当てたい画面表示幅</param>
        /// <param name="vSubjectDisplayWidth">科目欄に割り当てたい画面表示幅</param>
        /// <param name="vScoreDisplayWidth">点数欄に割り当てたい画面表示幅</param>
        /// <returns>出力書式の文字列</returns>
        private string CreateOutputFormat(StudentScore vStudentScore, int vNameDisplayWidth, int vSubjectDisplayWidth, int vScoreDisplayWidth) {

            int wNameFormatWidth = vNameDisplayWidth - CountZenkaku(vStudentScore.Name);
            int wSubjectFormatWidth = vSubjectDisplayWidth - CountZenkaku(vStudentScore.Subject);

            string[] wEachFormat = { $"{{0,-{wNameFormatWidth}}}", $"{{1,-{wSubjectFormatWidth}}}", $"{{2,{vScoreDisplayWidth}}}" }; // 点数は全角文字を含まないため、書式指定幅 = 画面表示幅
            return string.Join(" | ", wEachFormat);
        }

        /// <summary>
        /// コンソール画面で表示した時、半角何文字分の幅が必用かを計算するメソッド
        /// </summary>
        /// <param name="vText">調べたい文字列</param>
        /// <returns>画面表示時に必用な幅（半角換算）</returns>
        private int CalcDisplayWidth(string vText) {
            return Encoding.GetEncoding("shift_jis").GetByteCount(vText);
        }

        /// <summary>
        /// 文字列に含まれる全角文字の数を調べるメソッド
        /// </summary>
        /// <param name="vText">調べたい文字列</param>
        /// <returns>文字列に含まれる全角文字の数</returns>
        private int CountZenkaku(string vText) {
            return CalcDisplayWidth(vText) - vText.Length;
        }
    }
}