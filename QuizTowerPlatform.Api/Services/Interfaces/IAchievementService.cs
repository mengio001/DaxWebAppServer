using QuizTowerPlatform.Data.Context;
using QuizTowerPlatform.Data.DataModels;

namespace QuizTowerPlatform.Api.Services.Interfaces
{
    public interface IAchievementService
    {
        Task<IEnumerable<Achievement>> GetAllAchievements(IApiDbContext db);

        Task<IEnumerable<UserAchievement>> GetAchievementsByUser(IApiDbContext db, string username);
    }
}
