using Microsoft.EntityFrameworkCore;
using SimpleTradingApp.Domain.Entities;

namespace SimpleTradingApp.Infrastructure;

public class AppDbContext : DbContext
{
    public DbSet<Account> Accounts { get; set; }
    public DbSet<Trade> Trades { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) {}

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<Account>(entity =>
        {
            entity.HasKey(e => e.ID);
            entity.Property(e => e.FirstName).IsRequired();
            entity.Property(e => e.LastName).IsRequired();
            entity.HasMany(e => e.Trades).WithOne().HasForeignKey(t => t.AccountId);

            // Index on LastName for faster lookups
            entity.HasIndex(e => e.LastName);
        });
        modelBuilder.Entity<Trade>(entity =>
        {
            entity.HasKey(e => e.ID);
            entity.Property(e => e.SecurityCode).IsRequired();
            entity.Property(e => e.Timestamp).IsRequired();
            entity.Property(e => e.Amount).IsRequired();
            entity.Property(e => e.Type).IsRequired();
            entity.Property(e => e.Status).IsRequired();

            // Index on AccountId for faster lookups
            entity.HasIndex(e => e.AccountId);
        });
    }
}