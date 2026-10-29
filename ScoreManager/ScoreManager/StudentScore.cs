namespace ScoreManager {
    /// <summary>
    /// 生徒のデータを保持するクラス
    /// </summary>
    internal class StudentScore {
        /// <summary>
        /// 生徒の名前
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// テストの科目
        /// </summary>
        public string Subject { get; set; }

        /// <summary>
        /// テストの点数
        /// </summary>
        public int Score { get; set; }
        
        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="vName">設定する生徒の名前</param>
        /// <param name="vSubject">設定するテストの科目</param>
        /// <param name="vScore">設定するテストの点数</param>
        public StudentScore(string vName, string vSubject, int vScore) {
            this.Name = vName;
            this.Subject = vSubject;
            this.Score = vScore;
        }
    }
}