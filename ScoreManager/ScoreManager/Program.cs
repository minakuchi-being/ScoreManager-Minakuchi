using System;
using System.IO;

namespace ScoreManager {
    internal class Program {
        private const string C_DefaultLoadScoresFileName = "scores.csv";
        private const string C_DefaultExportScoresFileName = "output_scores.csv";

        static void Main(string[] args) {
            var wScoreManager = new ScoreManager();

            Console.WriteLine("=== 成績管理アプリ ===");
            Console.WriteLine(".exeと同階層にあるCSVファイルから成績データを読み込みます。");

            string wLoadFilePath = Path.Combine(AppContext.BaseDirectory, C_DefaultLoadScoresFileName);
            if (!wScoreManager.TryLoadCSV(wLoadFilePath, out LoadCSVStatusEnum wState)) {
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

            Console.WriteLine("=== 合格者一覧 ===");
            wScoreManager.DisplayPassingScores();
            Console.WriteLine();

            Console.WriteLine("=== 科目フィルタ ===");
            Console.Write("科目名を入力：");
            string wInput = Console.ReadLine()?.Trim();
            wScoreManager.DisplaySubjectFilteredScores(wInput);
            Console.WriteLine();

            Console.WriteLine("=== CSV出力 ===");
            string wExportFilePath = Path.Combine(AppContext.BaseDirectory, C_DefaultExportScoresFileName);
            wScoreManager.ExportCSV(wExportFilePath);
            Console.WriteLine();
        }
    }
}