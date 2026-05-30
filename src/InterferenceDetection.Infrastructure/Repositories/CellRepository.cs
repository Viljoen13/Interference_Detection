using InterferenceDetection.Application.Abstractions;
using InterferenceDetection.Domain.Entities;
using InterferenceDetection.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace InterferenceDetection.Infrastructure.Repositories;

public sealed class CellRepository : ICellRepository
{
    private readonly AppDbContext _db;

    public CellRepository(AppDbContext db)
    {
        _db = db;
    }

    public Task<Cell?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => _db.Cells.FirstOrDefaultAsync(c => c.Id == id, ct);

    public async Task<IReadOnlyList<Cell>> GetAllAsync(CancellationToken ct = default)
        => await _db.Cells.AsNoTracking().ToListAsync(ct);

    public async Task<IReadOnlyList<TCell>> GetByTechnologyAsync<TCell>(CancellationToken ct = default)
        where TCell : Cell
        => await _db.Cells.OfType<TCell>().AsNoTracking().ToListAsync(ct);

    public async Task AddAsync(Cell cell, CancellationToken ct = default)
    {
        await _db.Cells.AddAsync(cell, ct);
    }

    public async Task<bool> RemoveAsync(Guid id, CancellationToken ct = default)
    {
        var cell = await _db.Cells.FirstOrDefaultAsync(c => c.Id == id, ct);
        if (cell is null) return false;
        _db.Cells.Remove(cell);
        return true;
    }
}
