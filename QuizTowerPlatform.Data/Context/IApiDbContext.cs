using Microsoft.EntityFrameworkCore;
using QuizTowerPlatform.Data.DataModels;

namespace QuizTowerPlatform.Data.Context
{
    public interface IApiDbContext
    {
        DbSet<User> Users { get; }
        DbSet<Team> Teams { get; }
        DbSet<Quiz> Quizzes { get; }
        DbSet<Question> Questions { get; }
        DbSet<UserResult> UserResults { get; }
        DbSet<Achievement> Achievements { get; }
        DbSet<UserAchievement> UserAchievements { get; }
        DbSet<AspNetUser> AspNetUser { get; }
        DbSet<UserFacade> UserFacade { get; }
        DbSet<TotalAchievementPoints> TotalAchievementPoints { get; }

        void SetCurrentUsername(string username);
        int SaveChanges();
        int SaveChanges(bool acceptAllChangesOnSuccess);
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
