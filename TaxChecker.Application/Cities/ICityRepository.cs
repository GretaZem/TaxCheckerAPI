using TaxChecker.Domain;

namespace TaxChecker.Application.Cities;

public interface ICityRepository
{
    Task<List<City>> GetAllAsync(CancellationToken ct = default);
    Task<City?> GetByIdAsync(int cityId, CancellationToken ct = default);
    Task<City?> GetByNameAsync(string name, CancellationToken ct = default);
}