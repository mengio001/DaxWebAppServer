using QuizTowerPlatform.Data.DataModels;
using QuizTowerPlatform.Model;

namespace QuizTowerPlatform.Api.Models
{
    public class QuizServiceModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string QuizLogoUrl { get; set; }

        public ICollection<QuestionModel> QuizQuestions { get; set; }


        public List<AnswersServiceModel> Answers { get; set; }

        public UserResult Result { get; set; }
    }
}
