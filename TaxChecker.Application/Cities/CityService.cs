using TaxChecker.Domain;

namespace TaxChecker.Application.Cities;

public sealed class CityService : ICityService
{
    private readonly ICityRepository _repo;

    public CityService(ICityRepository repo)
    {
        _repo = repo;
    }

    public Task<List<City>> GetAllAsync(CancellationToken ct = default)
        => _repo.GetAllAsync(ct);
}
