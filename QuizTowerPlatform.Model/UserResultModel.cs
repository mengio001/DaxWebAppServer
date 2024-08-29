namespace QuizTowerPlatform.Model
{
    public class UserResultModel
    {

        public UserInfoModel User { get; set; }

        public QuizModel Quiz { get; set; }

        public int UsersCorrectAnswers { get; set; }

        public int UsersWrongAnswers { get; set; }

        public int PointsEarned { get; set; }
    }
}
