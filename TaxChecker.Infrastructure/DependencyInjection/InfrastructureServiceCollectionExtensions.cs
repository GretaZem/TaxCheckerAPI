using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TaxChecker.Application.Cities;
using TaxChecker.Application.Taxes;
using TaxChecker.Infrastructure.Data;
using TaxChecker.Infrastructure.Repositories;

namespace TaxChecker.Infrastructure.DependencyInjection;

public static class InfrastructureServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        string connectionString)
    {
        services.AddDbContext<AppDbContext>(options =>
        {
            options.UseNpgsql(connectionString);
        });

        services.AddScoped<ICityRepository, CityRepository>();
        services.AddScoped<ITaxRuleRepository, TaxRuleRepository>();

        return services;
    }
}
