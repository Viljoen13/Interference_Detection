using InterferenceDetection.Application.Abstractions;
using InterferenceDetection.Application.DTOs;

namespace InterferenceDetection.Api.Endpoints;

public static class AnalyticsEndpoints
{
    public static IEndpointRouteBuilder MapAnalyticsEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/analytics").WithTags("Analytics");

        group.MapGet("/distance", async (
            Guid cellA, Guid cellB,
            ICellRepository repo, IDistanceCalculator calc,
            CancellationToken ct) =>
        {
            var a = await repo.GetByIdAsync(cellA, ct);
            var b = await repo.GetByIdAsync(cellB, ct);
            if (a is null || b is null) return Results.NotFound();

            var distance = calc.DistanceMeters(a.Location, b.Location);
            return Results.Ok(new DistanceResponse(a.Id, b.Id, distance));
        })
        .WithName("GetDistance")
        .WithSummary("Great-circle (Haversine) distance between two cells in meters.");

        group.MapGet("/coverage-overlap", async (
            Guid cellA, Guid cellB,
            ICellRepository repo, ICoverageAnalyzer analyzer,
            CancellationToken ct) =>
        {
            var a = await repo.GetByIdAsync(cellA, ct);
            var b = await repo.GetByIdAsync(cellB, ct);
            if (a is null || b is null) return Results.NotFound();

            var result = analyzer.Analyze(a, b);
            return Results.Ok(new CoverageOverlapResponse(
                a.Id, b.Id, result.DistanceMeters, result.OverlapMeters, result.Overlaps));
        })
        .WithName("GetCoverageOverlap")
        .WithSummary("Coverage overlap (meters) between two cells, modeled as circles.");

        group.MapGet("/interference", async (
            ICellRepository repo, IInterferenceDetector detector,
            CancellationToken ct) =>
        {
            var cells = await repo.GetAllAsync(ct);
            var findings = detector.Detect(cells);
            var response = findings.Select(f => new InterferenceFindingResponse(
                f.CellAId, f.CellBId, f.Reason, f.DistanceMeters, f.OverlapMeters));
            return Results.Ok(response);
        })
        .WithName("GetInterferenceFindings")
        .WithSummary("Scan all cells and report potential RF interference pairs.");

        return app;
    }
}
