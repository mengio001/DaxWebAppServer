using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using QuizTower.IDP.Entities;

namespace QuizTowerPlatform.API.Entities
{
    [Table("Images", Schema = "Global")]
    public class Image : IConcurrencyAware
    {
        [Key]
        public Guid Id { get; set; }

        [ConcurrencyCheck]
        public string ConcurrencyStamp { get; set; } = Guid.NewGuid().ToString();

        [Required]
        [MaxLength(150)]
        public string Title { get; set; } = string.Empty;

        [Required]
        [MaxLength(200)]
        public string FileName { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string OwnerId { get; set; } = string.Empty;
    }
}
