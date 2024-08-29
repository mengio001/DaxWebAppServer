using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using QuizTowerPlatform.Data.Base;

namespace QuizTowerPlatform.Data.DataModels
{
    [Table($"{Constants.ViewPrefix + nameof(User)}", Schema = Constants.Schemas.QuizTowerPlatform)]
    public class User
    {
        [Key]
        public virtual int Id { get; set; }
        public virtual int TeamId { get; set; }
        public virtual string UserName { get; set; } = null!;
        public virtual string? Initials { get; set; }
        public virtual string? FirstName { get; set; }
        public virtual string? MiddleName { get; set; }
        public virtual string? LastName { get; set; }
        public virtual string? Email { get; set; }
        public virtual string? PhoneNumber { get; set; }
        public virtual DateTime? DateOfBirth { get; set; }
        public virtual int? UserId { get; set; } = null;

        [ForeignKey(nameof(TeamId))]
        public Team Team { get; set; } = null!;

        [InverseProperty("User")]
        public ICollection<UserAchievement> UserAchievements { get; set; } = new List<UserAchievement>();

        [InverseProperty("User")] 
        public virtual TotalAchievementPoints? TotalAchievementPoints { get; set; } = null;
    }
}