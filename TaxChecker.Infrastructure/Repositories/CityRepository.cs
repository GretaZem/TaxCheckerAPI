using Microsoft.EntityFrameworkCore;
using TaxChecker.Application.Cities;
using TaxChecker.Domain;
using TaxChecker.Infrastructure.Data;

namespace TaxChecker.Infrastructure.Repositories;

internal sealed class CityRepository : ICityRepository
{
    private readonly AppDbContext _db;

    public CityRepository(AppDbContext db)
    {
        _db = db;
    }

    public Task<List<City>> GetAllAsync(CancellationToken ct = default) =>
        _db.Cities.OrderBy(c => c.Name).ToListAsync(ct);

    public Task<City?> GetByIdAsync(int cityId, CancellationToken ct = default) =>
        _db.Cities.FirstOrDefaultAsync(c => c.Id == cityId, ct);

    public Task<City?> GetByNameAsync(string name, CancellationToken ct = default) =>
        _db.Cities.FirstOrDefaultAsync(c => c.Name == name, ct);
}