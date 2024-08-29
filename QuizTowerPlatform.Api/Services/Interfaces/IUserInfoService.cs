using QuizTowerPlatform.Api.Services.Security;
using QuizTowerPlatform.Data.Context;
using QuizTowerPlatform.Data.DataModels;
using QuizTowerPlatform.Model;

namespace QuizTowerPlatform.Api.Services.Interfaces
{
    public interface IUserInfoService
    {
        Task<UserInfoModel> ReadUserInfoAsync(IApiDbContext db, ICurrentLoggedInUser currentLoggedInUser);
        Task<IEnumerable<User>> GetUsersByTotalPoints(IApiDbContext db);
    }
}
