using Microsoft.EntityFrameworkCore;
using QuizTowerPlatform.Server.Entities;

namespace QuizTowerPlatform.Server.DbContexts
{
    public class ApiDbContext : DbContext, IApiDbContext
    { 
        public ApiDbContext(DbContextOptions<ApiDbContext> options) : base(options)
        {
        }

        public DbSet<Image> Images { get; set; } = null!;
        //public DbSet<User> Users { get; set; }

        //public DbSet<UserClaim> UserClaims { get; set; }

        //public DbSet<UserLogin> UserLogins { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //modelBuilder.Entity<User>()
            //.HasIndex(u => u.Subject)
            //.IsUnique();

            //modelBuilder.Entity<User>()
            //.HasIndex(u => u.UserName)
            //.IsUnique();

            //modelBuilder.Entity<User>().HasData(
            //    new User()
            //    {
            //        Id = new Guid("13229d33-99e0-41b3-b18d-4f72127e3971"),
            //        Password = "AQAAAAIAAYagAAAAEAFfhxfb2YqaRx4WePWJMkIE/tmk/oY7csVwmRqu63+TjAVYgulpGORreroxJD1AdA==",
            //        Subject = "d860efca-22d9-47fd-8249-791ba61b07c7",
            //        UserName = "David",
            //        Email = "david@someprovider.com",
            //        Active = true
            //    },
            //    new User()
            //    {
            //        Id = new Guid("96053525-f4a5-47ee-855e-0ea77fa6c55a"),
            //        Password = "AQAAAAIAAYagAAAAEAFfhxfb2YqaRx4WePWJMkIE/tmk/oY7csVwmRqu63+TjAVYgulpGORreroxJD1AdA==",
            //        Subject = "b7539694-97e7-4dfe-84da-b4256e1ff5c7",
            //        UserName = "Emma",
            //        Email = "emma@someprovider.com",
            //        Active = true
            //    });
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            // get updated entries
            var updatedConcurrencyAwareEntries = ChangeTracker.Entries()
                    .Where(e => e.State == EntityState.Modified)
                    .OfType<IConcurrencyAware>();

            foreach (var entry in updatedConcurrencyAwareEntries)
            {
                entry.ConcurrencyStamp = Guid.NewGuid().ToString();
            }

            return base.SaveChangesAsync(cancellationToken);
        }
    }
}
