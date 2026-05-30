using InterferenceDetection.Domain.Entities;

namespace InterferenceDetection.Application.Abstractions;

public interface ICellRepository
{
    Task<Cell?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<Cell>> GetAllAsync(CancellationToken ct = default);
    Task<IReadOnlyList<TCell>> GetByTechnologyAsync<TCell>(CancellationToken ct = default) where TCell : Cell;
    Task AddAsync(Cell cell, CancellationToken ct = default);
    Task<bool> RemoveAsync(Guid id, CancellationToken ct = default);
}
