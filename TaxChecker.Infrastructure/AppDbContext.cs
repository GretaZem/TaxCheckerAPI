using Microsoft.EntityFrameworkCore;
using TaxChecker.Domain;

namespace TaxChecker.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public DbSet<City> Cities => Set<City>();
    public DbSet<TaxRule> TaxRules => Set<TaxRule>();

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
