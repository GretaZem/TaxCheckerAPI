using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using System.Diagnostics;
using TaxChecker.Domain;
using TaxChecker.Infrastructure.Data;

namespace TaxChecker.Infrastructure.Database;

public static class DatabaseInitializer
{
    public static void EnsureDatabase(string connectionString)
    {
        EnsureDockerPostgresRunning();
        WaitForPostgres(connectionString);
    }

    private static void EnsureDockerPostgresRunning()
    {
        // Check if Docker is available
        var check = Process.Start(new ProcessStartInfo
        {
            FileName = "docker",
            Arguments = "--version",
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false
        });
        check!.WaitForExit();

        if (check.ExitCode != 0)
            throw new Exception("Docker is not installed or not available in PATH.");

        // Check if container exists
        var ps = Process.Start(new ProcessStartInfo
        {
            FileName = "docker",
            Arguments = "ps -a --filter \"name=taxapi-db\" --format \"{{.Names}}\"",
            RedirectStandardOutput = true,
            UseShellExecute = false
        });
        string output = ps!.StandardOutput.ReadToEnd().Trim();
        ps.WaitForExit();

        bool exists = output.Contains("taxapi-db");

        if (!exists)
        {
            Console.WriteLine("Database container not found. Starting with docker compose...");
            Process.Start(new ProcessStartInfo
            {
                FileName = "docker",
                Arguments = "compose up -d",
                WorkingDirectory = Directory.GetCurrentDirectory(),
                UseShellExecute = false
            })!.WaitForExit();
        }
        else
        {
            Console.WriteLine("Starting existing PostgreSQL container...");
            Process.Start("docker", "start taxapi-db")!.WaitForExit();
        }
    }

    private static void WaitForPostgres(string connectionString)
    {
        Console.WriteLine("Waiting for PostgreSQL to become available...");

        for (int i = 0; i < 20; i++)
        {
            try
            {
                using var conn = new NpgsqlConnection(connectionString);
                conn.Open();
                Console.WriteLine("PostgreSQL is up.");
                return;
            }
            catch
            {
                Thread.Sleep(1000);
            }
        }

        throw new Exception("PostgreSQL did not start in time.");
    }

    public static void ApplyMigrations(IServiceProvider provider)
    {
        Console.WriteLine("Applying EF Core migrations...");
        using var scope = provider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        db.Database.Migrate();
        Console.WriteLine("Migrations applied.");
    }

    public static void SeedData(string connectionString)
    {
        Console.WriteLine("Checking for seed data...");

        var services = new ServiceCollection();
        services.AddDbContext<AppDbContext>(o => o.UseNpgsql(connectionString));

        using var provider = services.BuildServiceProvider();
        using var scope = provider.CreateScope();

        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        // Check if already seeded
        if (db.Cities.Any()) return;

        Console.WriteLine("Seeding default test data...");

        var kaunas = new City("Kaunas");
        db.Cities.Add(kaunas);
        db.SaveChanges(); // need ID for FK

        var rules = new List<TaxRule>
    {
        new(kaunas.Id, TaxRuleType.Yearly, 3.3m,  new DateTime(2024,01,01), new DateTime(2024,12,31)),

        new(kaunas.Id, TaxRuleType.Monthly, 5m,   new DateTime(2024,06,01), new DateTime(2024,06,30)),
        new(kaunas.Id, TaxRuleType.Monthly, 4m,   new DateTime(2024,07,01), new DateTime(2024,07,31)),
        new(kaunas.Id, TaxRuleType.Monthly, 6m,   new DateTime(2024,08,01), new DateTime(2024,08,31)),

        new(kaunas.Id, TaxRuleType.Weekly, 2.5m,  new DateTime(2024,02,09), new DateTime(2024,02,15)),
        new(kaunas.Id, TaxRuleType.Weekly, 2.5m,  new DateTime(2024,03,02), new DateTime(2024,03,08)),

        new(kaunas.Id, TaxRuleType.Daily, 1.5m,   new DateTime(2024,06,01), new DateTime(2024,06,01)),
        new(kaunas.Id, TaxRuleType.Daily, 1.2m,   new DateTime(2024,10,23), new DateTime(2024,10,23)),
    };

        db.TaxRules.AddRange(rules);
        db.SaveChanges();

        Console.WriteLine("Seeding completed.");
    }
}
