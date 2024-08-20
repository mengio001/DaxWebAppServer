using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizTowerPlatform.Data.DataModels
{
    [Table("vw_" + nameof(UserAccount), Schema = Constants.Schemas.QuizTowerPlatform)]
    public class UserAccount
    {
        [Key]
        public int Id { get; set; }
        public string SubjectId { get; set; }
        public string IdentityProviderName { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
    }
}
