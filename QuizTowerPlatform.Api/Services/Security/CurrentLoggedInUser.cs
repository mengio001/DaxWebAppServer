using QuizTowerPlatform.Data.Context;
using QuizTowerPlatform.Data.DataModels;
using QuizTowerPlatform.Data.Types;

namespace QuizTowerPlatform.Api.Services.Security
{

    // TODO: rename Class name 'CurrentLoggedInUser' to InteractiveUser: Session-Based, user interacts directly with the system, executing commands or performing tasks that require immediate feedback.  
    public interface ICurrentLoggedInUser
    {
        public int Id { get; }
        public int TeamId { get; }
        public TeamType TeamType { get; }
        public string Username { get; }
        public IQueryable<User> AccessibleUsers(IApiDbContext db);
        public IQueryable<UserFacade> AccessibleUserByClaimTypeSub(IApiDbContext db, string sub);
    }

    public class CurrentLoggedInUser : ICurrentLoggedInUser
    {
        public CurrentLoggedInUser(int id, int teamId, TeamType teamType)
        {
            if (teamType is not (TeamType.TeamRed or TeamType.TeamBlue))
                throw new ArgumentOutOfRangeException(nameof(teamType), teamType, "Users within this TeamType are not supported");

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
            // TODO: AccessibleUsers for Quiz specific, this is not yet finished. 
            //switch (TeamType)
            //{
            //    case TeamType.TeamBlue:
            //        return db.Users.Where(u => u.Id == Id);
            //    default:
            //        throw new ArgumentOutOfRangeException(nameof(TeamType), TeamType, "Users within this TeamType are not supported");
            //}

            return db.Users.Where(u => u.Id == this.Id);
        }

        public IQueryable<UserFacade> AccessibleUserByClaimTypeSub(IApiDbContext db, string sub)
        {
            return db.UserFacade.Where(uf => uf.AspUserGuid.ToString() == sub);
        }
    }
}
