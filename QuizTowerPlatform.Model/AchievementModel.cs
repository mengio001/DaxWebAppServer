﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizTowerPlatform.Model
{
    public class AchievementModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Requirement { get; set; }

        public string LogoUrl { get; set; }

        public int Points { get; set; }

        // public List<UserAchievementModel> UserAchievements { get; set; } = new List<UserAchievementModel>();
    }
}
