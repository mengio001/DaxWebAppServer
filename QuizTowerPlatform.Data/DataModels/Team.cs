using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizTowerPlatform.Data.DataModels
{
    [Table("vw_" + nameof(Team), Schema = Constants.Schemas.QuizTowerPlatform)]
    public class Team
    {
        [Key]
        public int Id { get; set; }
        public string Type { get; set; }

        public ICollection<User> Users { get; set; } = null!;
    }
}
