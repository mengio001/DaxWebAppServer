using Microsoft.EntityFrameworkCore;
using QuizTowerPlatform.Api.Services.Interfaces;
using QuizTowerPlatform.Api.Services.Security;
using QuizTowerPlatform.Data.Context;
using QuizTowerPlatform.Data.DataModels;
using QuizTowerPlatform.Model;

namespace QuizTowerPlatform.Api.Services.Implementations
{
    public class UserInfoService : IUserInfoService
    {
        public async Task<UserInfoModel> ReadUserInfoAsync(IApiDbContext db, ICurrentLoggedInUser loggedInUser)
        {
            // TODO: Select child entity for AchievementPoints
            return await loggedInUser.AccessibleUsers(db).Where(u => u.Id == loggedInUser.Id)
                .Select(u => new UserInfoModel
                {
                    Username = u.UserName,
                    Initials = u.Initials,
                    MiddleName = u.MiddleName,
                    LastName = u.LastName,
                    TeamId = u.TeamId
                })
                .SingleOrDefaultAsync();
        }

        public async Task<IEnumerable<User>> GetUsersByTotalPoints(IApiDbContext db)
        {
            var users = await db.Users
                .Include(u => u.TotalAchievementPoints)
                .OrderByDescending(u => u.TotalAchievementPoints != null ? u.TotalAchievementPoints.TotalQuizPoints : 0)
                .ThenByDescending(u => u.TotalAchievementPoints != null ? u.TotalAchievementPoints.TotalAchievedPoints : 0)
                .ToListAsync();

            return users;
        }
    }
}
