using AutoMapper;
using Microsoft.EntityFrameworkCore;
using QuizTowerPlatform.Api.Services.Interfaces;
using QuizTowerPlatform.Api.Services.Security;
using QuizTowerPlatform.Data.Context;
using QuizTowerPlatform.Data.DataModels;
using QuizTowerPlatform.Model;
using System.Linq;

namespace QuizTowerPlatform.Api.Services.Implementations
{
    public class UserResultService : IUserResultService
    {
        private readonly IMapper mapper;

        public UserResultService(IMapper mapper)
        {
            this.mapper = mapper;
        }

        public async Task<UserResult> GetUserResultById(IApiDbContext db, int id, string username)
        {
            var user = await db.Users.FirstOrDefaultAsync(u => u.UserName == username);

            var userResult = await db.UserResults.Include(q => q.Quiz).FirstOrDefaultAsync(ur => ur.Id == id);

            return userResult;
        }

        public async Task<IEnumerable<UserResult>> GetAllUserResultsByUser(IApiDbContext db, string username)
        {
            return await db.UserResults
                .Include(x => x.User)
                .Include(x => x.Quiz)
                .Where(ur => ur.User.UserName.ToLower() == username.ToLower())
                .OrderByDescending(x => x)
                .ToListAsync();
        }
    }
}
