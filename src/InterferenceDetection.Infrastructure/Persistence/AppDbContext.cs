using InterferenceDetection.Application.Abstractions;
using InterferenceDetection.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace InterferenceDetection.Infrastructure.Persistence;

public sealed class AppDbContext : DbContext, IUnitOfWork
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Cell> Cells => Set<Cell>();
    public DbSet<LteCell> LteCells => Set<LteCell>();
    public DbSet<FiveGCell> FiveGCells => Set<FiveGCell>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}
