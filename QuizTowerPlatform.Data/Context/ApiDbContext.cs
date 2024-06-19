using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using QuizTowerPlatform.Data.Base;
using QuizTowerPlatform.Data.DataModels;

namespace QuizTowerPlatform.Data.Context
{
    public class ApiDbContext : DbContext, IApiDbContext
    {
        private string currentUsername;
        public ApiDbContext(DbContextOptions<ApiDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<UserAccount> UserAccounts { get; set; }
        public DbSet<Team> Teams { get; set; }

        public void SetCurrentUsername(string username)
        {
            currentUsername = username;
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            var now = DateTime.Now;
            ChangeTracker.Entries<ConcurrencyAware>().Where(e => e.State == EntityState.Added || e.State == EntityState.Modified).ToList().ForEach(entity =>
            {
                if (entity.State == EntityState.Added)
                {
                    entity.Entity.CreatedBy = currentUsername;
                    entity.Entity.Created = now;
                }
                else
                {
                    entity.Property(x => x.Created).IsModified = false;
                    entity.Property(x => x.CreatedBy).IsModified = false;
                }
                entity.Entity.ChangedBy = currentUsername;
                entity.Entity.Changed = now;
                entity.Entity.ConcurrencyStamp = Guid.NewGuid().ToString();
            });

            //// get updated entries
            //var updatedConcurrencyAwareEntries = ChangeTracker.Entries()
            //    .Where(e => e.State == EntityState.Modified)
            //    .OfType<ConcurrencyAware>();

            //foreach (var entry in updatedConcurrencyAwareEntries)
            //{
            //    entry.ConcurrencyStamp = Guid.NewGuid().ToString();
            //}

            return base.SaveChangesAsync(cancellationToken);
        }
    }
}
