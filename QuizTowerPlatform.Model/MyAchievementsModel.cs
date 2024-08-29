namespace QuizTowerPlatform.Model
{
    public class MyAchievementsModel
    {
        public int UserId { get; set; }

        public UserInfoModel User { get; set; }

        public int AchievementId { get; set; }

        public AchievementModel Achievement { get; set; }

        public DateTime AchievedOn { get; set; }
    }
}
