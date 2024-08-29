using QuizTowerPlatform.Data.Types;

namespace QuizTowerPlatform.Model
{
    public class QuizModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public QuizGenre Category { get; set; }

        public string QuizLogoUrl { get; set; }

        public ICollection<QuestionModel> QuizQuestions { get; set; }


        public List<AnswerModel> Answers { get; set; }

        public UserResultModel Result { get; set; }

    }
}
