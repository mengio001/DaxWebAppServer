using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using QuizTowerPlatform.Data.Base;
using QuizTowerPlatform.Data.DataModels;

namespace QuizTowerPlatform.Data.Context
{
    public class ApiDbContext : DbContext, IApiDbContext
    {
        private readonly IConfiguration _configuration;
        private string _currentUsername;

        public ApiDbContext(DbContextOptions<ApiDbContext> options) : base(options) { }
        public DbSet<Quiz> Quizzes { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<UserResult> UserResults { get; set; }
        public DbSet<Achievement> Achievements { get; set; }
        public DbSet<UserAchievement> UserAchievements { get; set; }
        public DbSet<AspNetUser> AspNetUser { get; set; } = null!;
        public DbSet<UserFacade> UserFacade { get; set; } = null!;
        public DbSet<User> Users { get; set; }
        public DbSet<Team> Teams { get; set; }
        public DbSet<TotalAchievementPoints> TotalAchievementPoints { get; set; }
        
        public void SetCurrentUsername(string username)
        {
            _currentUsername = username;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            if (!options.IsConfigured)
            {
                options.UseSqlServer(_configuration.GetConnectionString("DefaultConnection"));
            }
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            PopulateAuditValues();
            return base.SaveChangesAsync(cancellationToken);
        }

        public override int SaveChanges()
        {
            PopulateAuditValues();
            return base.SaveChanges();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Ignore dynamically named views with prefix 'vw_'
            var typesToIgnore = typeof(ApiDbContext).Assembly.GetTypes()
                .Where(t => t.Name.StartsWith(Constants.ViewPrefix, StringComparison.OrdinalIgnoreCase));

            foreach (var type in typesToIgnore)
            {
                modelBuilder.Ignore(type);
            }

            // Ignore Views to prevent EF from creating it as a table
            modelBuilder.Ignore<AspNetUser>();
            modelBuilder.Ignore<UserFacade>();
            modelBuilder.Ignore<User>();
            modelBuilder.Ignore<Team>();

            var isMigration = Environment.GetEnvironmentVariable("MIGRATION")?.ToLower() == "true";

            // Note: Disable the next two lines before creating an EF migration by setting the environment variable MIGRATION to true.
            // set MIGRATION=[true|false]{0,1} || $env:MIGRATION = "true" || Remove-Item Env:MIGRATION
            // echo %MIGRATION% || echo $env:MIGRATION
            if (!isMigration)
            {
                modelBuilder.Entity<AspNetUser>().HasKey(u => new { u.Id });
                modelBuilder.Entity<UserFacade>().HasKey(uf => new { uf.UserId });
                modelBuilder.Entity<User>().HasKey(u => new { u.Id });
                modelBuilder.Entity<Team>().HasKey(t => new { t.Id });
            }

            // Relationship configurations
            ConfigureRelationships(modelBuilder);

            // Seed initial data
            SeedDatabase(modelBuilder);
        }

        private void ConfigureRelationships(ModelBuilder modelBuilder)
        {
        }

        private void SeedDatabase(ModelBuilder modelBuilder)
        {
        }

        private void PopulateAuditValues()
        {
            _currentUsername ??= Constants.Schemas.QuizTowerPlatform;
            var now = DateTime.Now;

            ChangeTracker.Entries<AuditableEntity>().Where(e => e.State == EntityState.Added || e.State == EntityState.Modified).ToList()
                .ForEach(entity =>
                {
                    if (entity.State == EntityState.Added)
                    {
                        entity.Entity.CreatedBy = _currentUsername;
                        entity.Entity.Created = now;
                    }
                    else
                    {
                        entity.Property(x => x.Created).IsModified = false;
                        entity.Property(x => x.CreatedBy).IsModified = false;
                    }
                    entity.Entity.ChangedBy = _currentUsername;
                    entity.Entity.Changed = now;
                    entity.Entity.ConcurrencyStamp = Guid.NewGuid().ToString();
                });
        }
    }
}
