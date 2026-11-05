using System;
using System.IO;

namespace ScoreManager {
    internal class Program {
        private const string C_DefaultScoreFileName = "scores.csv";

        static void Main(string[] args) {
            var wScoreManager = new ScoreManager();

            Console.WriteLine("=== 成績管理アプリ ===");
            Console.WriteLine(".exeと同階層にあるCSVファイルから成績データを読み込みます。");

            string wFilePath = Path.Combine(AppContext.BaseDirectory, C_DefaultScoreFileName);
            if (!wScoreManager.TryLoadCSV(wFilePath, out LoadCSVStatusEnum wState)) {
                switch (wState) {
                    case LoadCSVStatusEnum.FileNotFound:
                        Console.WriteLine("指定されたファイルが存在しません。");
                        Console.WriteLine("アプリケーションを終了します。");
                        return;
                    case LoadCSVStatusEnum.NoValidScore:
                        Console.WriteLine("有効な成績が1件も存在しません。");
                        Console.WriteLine("アプリケーションを終了します。");
                        return;
                }
            }
            Console.WriteLine();

            Console.WriteLine("=== 成績一覧 ===");
            wScoreManager.DisplayAllScores();
            Console.WriteLine();

            Console.WriteLine("=== 全教科平均点 ===");
            wScoreManager.DisplayOverallAverageScore();
            Console.WriteLine();

            Console.WriteLine("=== 科目別平均点 ===");
            wScoreManager.DisplaySubjectAverageScores();
            Console.WriteLine();
        }
    }
}