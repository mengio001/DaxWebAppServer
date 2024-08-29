using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QuizTowerPlatform.Data.Types;

namespace QuizTowerPlatform.Data.DataModels
{
    // TODO: The implementation is not finished yet; there is currently no option to select a user within a team.

    [Table($"{Constants.ViewPrefix + nameof(Team)}", Schema = Constants.Schemas.QuizTowerPlatform)]
    public class Team
    {
        [Key]
        public int Id { get; set; }
        public string? TeamTypeString { get; set; }

        [NotMapped]
        public TeamType? TeamType => TeamTypeString.ToTeamType();

        public int UserId { get; set; }

        [ForeignKey(nameof(UserId))]
        public ICollection<User> Users { get; set; } = null!;
    }
}
