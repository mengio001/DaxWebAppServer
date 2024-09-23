using QuizTowerPlatform.Data.Base;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuizTowerPlatform.Data.DataModels
{
    [Table(nameof(TotalAchievementPoints), Schema = Constants.Schemas.QuizTowerPlatform)]
    public class TotalAchievementPoints : AuditableEntity
    {
        public int TotalQuizPoints { get; set; } = 0;

        public int TotalAchievedPoints { get; set; } = 0;

        [Required]
        public int UserId { get; set; }

        [ForeignKey(nameof(UserId))]
        [InverseProperty("TotalAchievementPoints")]
        public User User { get; set; }
    }
}
