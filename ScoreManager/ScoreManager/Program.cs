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

            while (true) {
                Console.WriteLine("=== 成績管理メニュー ===");
                Console.WriteLine("1. 一覧表示");
                Console.WriteLine("2. 平均点表示");
                Console.WriteLine("3. 科目別平均点表示");
                Console.WriteLine("4. 合格者一覧");
                Console.WriteLine("5. 科目でフィルタ");
                Console.WriteLine("6. CSV出力");
                Console.WriteLine("0. 終了");

                Console.Write("選択：");
                string wMenuInput = Console.ReadLine()?.Trim();
                Console.WriteLine();

                switch (wMenuInput) {
                    case "1":
                        Console.WriteLine("=== 成績一覧 ===");
                        wScoreManager.DisplayAllScores();
                        break;

                    case "2":
                        Console.WriteLine("=== 全教科平均点 ===");
                        wScoreManager.DisplayOverallAverageScore();
                        break;

                    case "3":
                        Console.WriteLine("=== 科目別平均点 ===");
                        wScoreManager.DisplaySubjectAverageScores();
                        break;

                    case "4":
                        Console.WriteLine("=== 合格者一覧 ===");
                        wScoreManager.DisplayPassingScores();
                        break;

                    case "5":
                        Console.WriteLine("=== 科目フィルタ ===");
                        Console.Write("科目名を入力：");
                        string wSubjectInput = Console.ReadLine()?.Trim();
                        wScoreManager.DisplaySubjectFilteredScores(wSubjectInput);
                        break;

                    case "6":
                        Console.WriteLine("=== CSV出力 ===");
                        string wExportFilePath = Path.Combine(AppContext.BaseDirectory, C_DefaultExportScoresFileName);
                        wScoreManager.ExportCSV(wExportFilePath);
                        break;

                    case "0":
                        Console.WriteLine("アプリケーションを終了します。");
                        return;

                    default:
                        Console.WriteLine("無効な入力です。");
                        Console.WriteLine("半角でメニュー番号を入力してください。");
                        break;
                }
                Console.WriteLine();
            }
        }
    }
}