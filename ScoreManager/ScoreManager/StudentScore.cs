using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScoreManager {
    internal class StudentScore {

        public string Name { get; set; }        // 名前
        public string Subject { get; set; }     // 科目
        public int Score { get; set; }          // 点数


        public StudentScore(string vName, string vSubject, int vScore) {
            Name = vName;
            Subject = vSubject;
            Score = vScore;
        }
}
