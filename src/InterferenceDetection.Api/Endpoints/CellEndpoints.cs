using InterferenceDetection.Application.Abstractions;
using InterferenceDetection.Application.DTOs;
using InterferenceDetection.Application.Mapping;
using InterferenceDetection.Domain.Entities;
using InterferenceDetection.Domain.ValueObjects;

namespace InterferenceDetection.Api.Endpoints;

public static class CellEndpoints
{
    public static IEndpointRouteBuilder MapCellEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/cells").WithTags("Cells");

        group.MapGet("/", async (ICellRepository repo, CancellationToken ct) =>
        {
            var cells = await repo.GetAllAsync(ct);
            return Results.Ok(cells.Select(CellMapper.ToDto));
        })
        .WithName("ListCells")
        .WithSummary("List all cells (LTE and 5G).");

        group.MapGet("/{id:guid}", async (Guid id, ICellRepository repo, CancellationToken ct) =>
        {
            var cell = await repo.GetByIdAsync(id, ct);
            return cell is null ? Results.NotFound() : Results.Ok(CellMapper.ToDto(cell));
        })
        .WithName("GetCell")
        .WithSummary("Get a single cell by ID.");

        group.MapGet("/lte", async (ICellRepository repo, CancellationToken ct) =>
        {
            var cells = await repo.GetByTechnologyAsync<LteCell>(ct);
            return Results.Ok(cells.Select(c => (CellDto)CellMapper.ToDto(c)));
        })
        .WithName("ListLteCells")
        .WithSummary("List LTE cells only.");

        group.MapGet("/5g", async (ICellRepository repo, CancellationToken ct) =>
        {
            var cells = await repo.GetByTechnologyAsync<FiveGCell>(ct);
            return Results.Ok(cells.Select(c => (CellDto)CellMapper.ToDto(c)));
        })
        .WithName("ListFiveGCells")
        .WithSummary("List 5G cells only.");

        group.MapPost("/lte", async (CreateLteCellRequest req, ICellRepository repo, IUnitOfWork uow, CancellationToken ct) =>
        {
            var cell = new LteCell(
                Guid.NewGuid(), req.Name,
                new GeoCoordinate(req.Latitude, req.Longitude),
                req.CoverageRadiusMeters, req.TransmitPowerDbm,
                req.Earfcn, req.PhysicalCellId, req.TrackingAreaCode);

            await repo.AddAsync(cell, ct);
            await uow.SaveChangesAsync(ct);

            return Results.Created($"/cells/{cell.Id}", CellMapper.ToDto(cell));
        })
        .WithName("CreateLteCell")
        .WithSummary("Create a new LTE cell.");

        group.MapPost("/5g", async (CreateFiveGCellRequest req, ICellRepository repo, IUnitOfWork uow, CancellationToken ct) =>
        {
            var cell = new FiveGCell(
                Guid.NewGuid(), req.Name,
                new GeoCoordinate(req.Latitude, req.Longitude),
                req.CoverageRadiusMeters, req.TransmitPowerDbm,
                req.Nrarfcn, req.PhysicalCellId, req.GnbId, req.TrackingAreaCode);

            await repo.AddAsync(cell, ct);
            await uow.SaveChangesAsync(ct);

            return Results.Created($"/cells/{cell.Id}", CellMapper.ToDto(cell));
        })
        .WithName("CreateFiveGCell")
        .WithSummary("Create a new 5G cell.");

        group.MapDelete("/{id:guid}", async (Guid id, ICellRepository repo, IUnitOfWork uow, CancellationToken ct) =>
        {
            var removed = await repo.RemoveAsync(id, ct);
            if (!removed) return Results.NotFound();
            await uow.SaveChangesAsync(ct);
            return Results.NoContent();
        })
        .WithName("DeleteCell")
        .WithSummary("Delete a cell by ID.");

        return app;
    }
}
