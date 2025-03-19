using Microsoft.EntityFrameworkCore;

public class CacheDbContext : DbContext
{
    public DbSet<CacheEntry> CacheEntries { get; set; }

    public CacheDbContext(DbContextOptions<CacheDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CacheEntry>().HasKey(c => c.Parameters);
    }
}
