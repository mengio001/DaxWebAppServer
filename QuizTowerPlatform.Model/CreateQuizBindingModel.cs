using System.ComponentModel.DataAnnotations;
using QuizTowerPlatform.Data.Types;

namespace QuizTowerPlatform.Model
{
    public class CreateQuizBindingModel
    {

        [Required(ErrorMessage = "Name is required and must be maximum 20 symbols!")]
        [StringLength(20)]
        public string Name { get; set; }

        [Required]
        public QuizGenre Category { get; set; }

        [Required(ErrorMessage = "Logo is required!")]
        [Url]
        public string QuizLogoUrl { get; set; }
    }
}
