using QuizTowerPlatform.Data.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizTowerPlatform.Data.DataModels
{
    [Table(nameof(UserAchievement), Schema = Constants.Schemas.QuizTowerPlatform)]
    public class UserAchievement : AuditableEntity
    {
        public int UserId { get; set; }

        [ForeignKey(nameof(UserId))]
        [InverseProperty("UserAchievements")]
        public User User { get; set; }

        public int AchievementId { get; set; }

        [ForeignKey(nameof(AchievementId))]
        [InverseProperty("UserAchievements")]
        public Achievement Achievement { get; set; }

        public DateTime AchievedOn { get; set; }
    }
}