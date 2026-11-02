namespace ScoreManager {
    /// <summary>
    /// LoadCSVメソッドの終了時の状態を表す列挙型
    /// </summary>
    internal enum LoadCSVStatusEnum {
        /// <summary>
        /// 成功（1件以上の成績をリストに格納した）
        /// </summary>
        Success,

        /// <summary>
        /// ファイルが存在しない
        /// </summary>
        FileNotFound,

        /// <summary>
        /// 有効な成績が1件も存在しない
        /// </summary>
        NoValidScore
    }
}