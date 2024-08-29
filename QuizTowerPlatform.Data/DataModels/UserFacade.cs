using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using QuizTowerPlatform.Data.Types;

namespace QuizTowerPlatform.Data.DataModels
{
    [Table($"{Constants.ViewPrefix + nameof(UserFacade)}", Schema = Constants.Schemas.TOQ)]
    public class UserFacade
    {
        [Key]
        public int UserId { get; set; }
        public Guid AspUserGuid { get; set; }

        [Required]
        public string UserName { get; set; }
        public string? FirstName { get; set; }
        public string? MiddleName { get; set; }
        public string? LastName { get; set; }
        [Column("Gender")]
        public string? GenderString { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public DateTime? DateOfBirth { get; set; }

        [NotMapped]
        public Gender? Gender => GenderString.ToBinaryGender();

        public bool IsDeleted { get; set; }
        public DateTime ValidFrom { get; set; }
        public DateTime ValidUntil { get; set; }
        public bool MembershipIsApproved { get; set; }
        public bool MembershipIsLockedOut { get; set; }
    }

    public static class UserFacadeExtensions
    {
        public static string FullName(this UserFacade user)
        {
            return string.IsNullOrWhiteSpace(user.MiddleName?.Trim())
                ? string.Format("{0} {1}", user.FirstName?.Trim(), user.LastName?.Trim())
                : string.Format("{0} {1} {2}", user.FirstName?.Trim(), user.MiddleName?.Trim(), user.LastName?.Trim());
        }
    }
}
