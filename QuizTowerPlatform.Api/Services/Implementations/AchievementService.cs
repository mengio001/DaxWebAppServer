using AutoMapper;
using Microsoft.EntityFrameworkCore;
using QuizTowerPlatform.Api.Services.Interfaces;
using QuizTowerPlatform.Data.Context;
using QuizTowerPlatform.Data.DataModels;

namespace QuizTowerPlatform.Api.Services.Implementations
{
    public class AchievementService : IAchievementService
    {
        public async Task<IEnumerable<Achievement>> GetAllAchievements(IApiDbContext db)
        {
            return await db.Achievements.ToListAsync();
        }

        public async Task<IEnumerable<UserAchievement>> GetAchievementsByUser(IApiDbContext db, string username)
        {
            return await db.UserAchievements
                .Where(x => x.User.UserName.Equals(username))
                .Include(x=>x.Achievement)
                .OrderByDescending(x=>x.AchievedOn)
                .ToListAsync();
        }
    }
}
