using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using QuizTowerPlatform.Data.Types;

namespace QuizTowerPlatform.Data.DataModels
{
    [Table($"{Constants.ViewPrefix + nameof(AspNetUser)}", Schema = Constants.Schemas.TOQ)]
    public class AspNetUser
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public virtual Guid Id { get; set; }

        [Required]
        [MaxLength(200)]
        public virtual string SubjectId { get; set; }

        [MaxLength(200)]
        public virtual string? UserName { get; set; }

        public virtual string IdentityProviderName { get; set; }

        public virtual string? Password { get; set; }

        public virtual bool Active { get; set; }

        public virtual string? Email { get; set; }
    }
}