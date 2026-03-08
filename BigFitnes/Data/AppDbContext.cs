using BigFitnes.Models;
using Microsoft.EntityFrameworkCore;

namespace BigFitnes.Data;

public class AppDbContext : DbContext
{
    public DbSet<Product> Products => Set<Product>();
    public DbSet<FoodEntry> FoodEntries => Set<FoodEntry>();
    public DbSet<WeightRecord> WeightRecords => Set<WeightRecord>();
    public DbSet<UserProfile> UserProfiles => Set<UserProfile>();

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<FoodEntry>()
            .HasOne(f => f.Product)
            .WithMany()
            .HasForeignKey(f => f.ProductId);
    }
}
