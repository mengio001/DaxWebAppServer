using Microsoft.EntityFrameworkCore;
using QuizTowerPlatform.API.Entities;

namespace QuizTowerPlatform.API.DbContexts;

public class ApiDbContext : DbContext
{
    public ApiDbContext(DbContextOptions<ApiDbContext> options)
        : base(options)
    {
    }

    public DbSet<Image> Images { get; set; } = null!;
}