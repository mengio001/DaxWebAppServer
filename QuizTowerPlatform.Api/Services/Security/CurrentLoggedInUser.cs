using QuizTowerPlatform.Data.Context;
using QuizTowerPlatform.Data.DataModels;
using QuizTowerPlatform.Data.Types;
using System.Security.Principal;

namespace QuizTowerPlatform.Api.Services.Security
{

    public interface ICurrentLoggedInUser
    {
        public int Id { get; }
        public int TeamId { get; }
        public TeamType TeamType { get; }
        public string Username { get; }
        public IQueryable<User> AccessibleUsers(IApiDbContext db);
        
    }

    public class CurrentLoggedInUser : ICurrentLoggedInUser
    {
        public CurrentLoggedInUser(int id, int teamId, TeamType teamType)
        {
            if (teamType is not (TeamType.TeamRed or TeamType.TeamYellow))
                throw new ArgumentOutOfRangeException(nameof(teamType), teamType, "Gebruikers binnen deze TeamType worden niet ondersteund");

            Id = id;
            TeamId = teamId;
            TeamType = teamType;
        }

        public int Id { get; }
        public int TeamId { get; }
        public TeamType TeamType { get; }
        public string Username => "DummyUsername";

        public IQueryable<User> AccessibleUsers(IApiDbContext db)
        {
            switch (TeamType)
            {
                case TeamType.TeamBlue:
                    return db.Users.Where(g => g.Id == Id);
                
                default:
                    throw new ArgumentOutOfRangeException(nameof(TeamType), TeamType, "Gebruikers binnen deze TeamType worden niet ondersteund");
            }
        }
    }
}
