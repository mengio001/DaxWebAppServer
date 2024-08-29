using QuizTowerPlatform.Data.Context;
using QuizTowerPlatform.Data.DataModels;

namespace QuizTowerPlatform.Api.Services.Interfaces
{
    public interface IUserResultService
    {
        Task<IEnumerable<UserResult>> GetAllUserResultsByUser(IApiDbContext db, string username);
        Task<UserResult> GetUserResultById(IApiDbContext db, int id, string username);
    }
}
