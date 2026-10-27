using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace ScoreManager {
    /// <summary>
    /// 生徒のデータを扱う処理を担当するクラス
    /// </summary>
    internal class ScoreManager {
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

                    // --- 空行・コメント行のスキップ ---
                    if (string.IsNullOrWhiteSpace(wLine) || wLine.TrimStart().StartsWith("#")) {
                        continue;
                    }

                    string[] wData = wLine.Split(',');
                    
                    // --- 文字列のバリデーションチェック ---
                    // 項目数が不正
                    if (wData.Length != 3) {
                        Console.WriteLine($"不正な行（項目数不正）: {wLine}");
                        continue;
                    }
                    // 名前が不正（ひらがな / カタカナ / 漢字 / 英字 のみ可）
                    if (!Regex.IsMatch(wData[0].Trim(), @"^[\p{IsHiragana}\p{IsKatakana}\p{IsCJKUnifiedIdeographs}a-zA-Z]+$")) {
                        Console.WriteLine($"名前が不正: {wData[0]}");
                        continue;
                    }
                    // 科目が不正（ひらがな / カタカナ / 漢字 のみ可）
                    if (!Regex.IsMatch(wData[1].Trim(), @"^[\p{IsHiragana}\p{IsKatakana}\p{IsCJKUnifiedIdeographs}]+$")) {
                        Console.WriteLine($"科目が不正: {wData[1]}");
                        continue;
                    }
                    // 成績が不正（0～100 の整数値 のみ可）
                    if (!Regex.IsMatch(wData[2].Trim(), @"^(100|[1-9]?[0-9])$")) {
                        Console.WriteLine($"成績が不正: {wData[2]}");
                        continue;
                    }
                    StudentScore wStudentScore = new StudentScore(wData[0].Trim(), wData[1].Trim(), int.Parse(wData[2].Trim()));
                    FStudentScores.Add(wStudentScore);
                }
            }
            Console.WriteLine($"{FStudentScores.Count} 件のデータを読み込みました。");
        }
    }
}