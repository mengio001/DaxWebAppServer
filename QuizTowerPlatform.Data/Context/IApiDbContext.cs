using Microsoft.EntityFrameworkCore;
using QuizTowerPlatform.Data.DataModels;

namespace QuizTowerPlatform.Data.Context
{
    public interface IApiDbContext
    {
        public DbSet<User> Users { get; }
        public DbSet<UserAccount> UserAccounts { get; }
        public DbSet<Team> Teams { get; }

        int SaveChanges();
        int SaveChanges(bool acceptAllChangesOnSuccess);
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken());
        void SetCurrentUsername(string username);
    }
}
