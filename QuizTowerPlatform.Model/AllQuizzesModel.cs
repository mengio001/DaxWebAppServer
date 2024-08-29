using QuizTowerPlatform.Data.Types;

namespace QuizTowerPlatform.Model
{
    public class AllQuizzesModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public QuizGenre Category { get; set; }

        public string QuizLogoUrl { get; set; }

        public List<QuestionModel> QuizQuestions { get; set; }
    }
}
