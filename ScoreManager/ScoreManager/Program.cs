using System;
using System.IO;

namespace ScoreManager {
    internal class Program {
        private const string C_DefaultScoreFileName = "scores.csv";

        static void Main(string[] args) {
            ScoreManager wScoreManager = new ScoreManager();

            Console.WriteLine("=== 成績管理アプリ ===");
            Console.WriteLine(".exeと同階層にあるCSVファイルから成績データを読み込みます。");

            string wFilePath = Path.Combine(AppContext.BaseDirectory, C_DefaultScoreFileName);
            wScoreManager.LoadCSV(wFilePath);
        }
    }
}