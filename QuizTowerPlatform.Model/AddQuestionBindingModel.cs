using System.ComponentModel.DataAnnotations;

namespace QuizTowerPlatform.Model
{
    public class AddQuestionBindingModel
    {
        [Required(ErrorMessage = "Question name is required and must be maximum 100 symbols!")]
        [StringLength(100)]
        public string QuestionName { get; set; }

        [Required(ErrorMessage = "Question's answer option is required and must be maximum 30 symbols!")]
        [StringLength(30)]
        public string FirstOption { get; set; }

        [Required(ErrorMessage = "Question's answer option is required and must be maximum 30 symbols!")]
        [StringLength(30)]
        public string SecondOption { get; set; }

        [Required(ErrorMessage = "Question's answer option is required and must be maximum 30 symbols!")]
        [StringLength(30)]
        public string ThirdOption { get; set; }

        [Required(ErrorMessage = "Question's answer option is required and must be maximum 30 symbols!")]
        [StringLength(30)]
        public string FourthOption { get; set; }

        [Required(ErrorMessage = "Question's correct answer is required and must be same as one of the options!")]
        [StringLength(100)]
        public string CorrectAnswer { get; set; }

        public int CorrectAnswerPoints { get; set; }

        public int QuizId { get; set; }

        //public QuizModel Quiz { get; set; }

        //public IEnumerable<QuizModel> Quizzes { get; set; }

    }
}
