using TaxChecker.Domain;

namespace TaxChecker.Application.Cities;

public interface ICityService
{
    Task<List<City>> GetAllAsync(CancellationToken ct = default);
}
