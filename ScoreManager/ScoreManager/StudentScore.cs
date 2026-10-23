using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScoreManager {
    public class StudentScore {

        public string Name { get; set; }
        public string Subject { get; set; }
        public int Score { get; set; }


        public StudentScore(string vName, string vSubject, int vScore) {
            Name = vName;
            Subject = vSubject;
            Score = vScore;
        }
    }
}
