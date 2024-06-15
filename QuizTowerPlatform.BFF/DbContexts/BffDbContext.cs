using Microsoft.EntityFrameworkCore;
using QuizTowerPlatform.BFF.Entities;

namespace QuizTowerPlatform.BFF.DbContexts;

public class BffDbContext : DbContext, IBffDbContext
{
    public BffDbContext(DbContextOptions<BffDbContext> options) : base(options)
    {
    }

    //public DbSet<Image> Images { get; set; } = null!;

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // get updated entries
        var updatedConcurrencyAwareEntries = ChangeTracker.Entries()
            .Where(e => e.State == EntityState.Modified)
            .OfType<IConcurrencyAware>();

        foreach (var entry in updatedConcurrencyAwareEntries) entry.ConcurrencyStamp = Guid.NewGuid().ToString();

        return base.SaveChangesAsync(cancellationToken);
    }
}