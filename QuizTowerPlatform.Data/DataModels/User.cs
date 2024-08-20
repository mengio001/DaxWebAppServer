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
    [Table("vw_" + nameof(User), Schema = Constants.Schemas.QuizTowerPlatform)]
    public class User
    {
        [Key]
        public int Id { get; set; }
        public int TeamId { get; set; }
        public string Username { get; set; } = null!;
        public string? Initials { get; set; }
        public string? FirstName { get; set; }
        public string? Prefixes { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public DateTime? DateOfBirth { get; set; }

        [ForeignKey(nameof(TeamId))]
        public Team Team { get; set; } = null!;
    }
}
