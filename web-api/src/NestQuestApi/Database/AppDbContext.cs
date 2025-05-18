using NestQuestApi.Models;
using Microsoft.EntityFrameworkCore;

namespace NestQuestApi.Database;

public class AppDbContext : DbContext
{
    public DbSet<CacheEntry> CacheEntries { get; set; }
    public DbSet<Criterion> Criteria { get; set; }
    public DbSet<Place> Places { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CacheEntry>().HasKey(c => c.Parameters);
        modelBuilder.Entity<Criterion>().HasKey(c => c.Id);
        modelBuilder.Entity<Place>().HasKey(c => c.Id);
    }
}
