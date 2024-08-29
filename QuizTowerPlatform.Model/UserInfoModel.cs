using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizTowerPlatform.Model
{
    public class UserInfoModel
    {
        public string Username { get; set; }
        public string Initials { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }

        public int TeamId { get; set; }

        public int TotalQuizPoints { get; set; }
        public int TotalAchievementPoints { get; set; }
    }
}
