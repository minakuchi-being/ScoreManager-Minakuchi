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
        public void LoadCSV(string vFilePath) {

            if (!File.Exists(vFilePath)) {
                Console.WriteLine($"{vFilePath} は存在していません。");
                return;
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
            Console.WriteLine($"{FStudentScores.Count} 件のデータを読み込みました。");
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
    }
}