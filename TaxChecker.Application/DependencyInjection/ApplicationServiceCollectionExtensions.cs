using Microsoft.Extensions.DependencyInjection;

namespace TaxChecker.Application.DependencyInjection;

public static class ApplicationServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<Taxes.ITaxService, Taxes.TaxService>();
        services.AddScoped<Cities.ICityService, Cities.CityService>();
        return services;
    }
}
