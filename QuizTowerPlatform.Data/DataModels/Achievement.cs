using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QuizTowerPlatform.Data.Base;

namespace QuizTowerPlatform.Data.DataModels
{

    [Table(nameof(Achievement), Schema = Constants.Schemas.QuizTowerPlatform)]
    public class Achievement: AuditableEntity
    {
        public string Name { get; set; }

        public string Requirement { get; set; }

        public string LogoUrl { get; set; }

        public int Points { get; set; }

        [InverseProperty("Achievement")]
        public ICollection<UserAchievement> UserAchievements { get; set; } = new List<UserAchievement>();
    }
}
