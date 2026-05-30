using InterferenceDetection.Application.Abstractions;
using InterferenceDetection.Domain.Entities;

namespace InterferenceDetection.Application.Services;

public sealed class FrequencyInterferenceDetector : IInterferenceDetector
{
    private readonly ICoverageAnalyzer _coverageAnalyzer;

    public FrequencyInterferenceDetector(ICoverageAnalyzer coverageAnalyzer)
    {
        _coverageAnalyzer = coverageAnalyzer;
    }

    public IReadOnlyList<InterferenceFinding> Detect(IReadOnlyList<Cell> cells)
    {
        ArgumentNullException.ThrowIfNull(cells);

        var findings = new List<InterferenceFinding>();

        for (var i = 0; i < cells.Count; i++)
        {
            for (var j = i + 1; j < cells.Count; j++)
            {
                var reason = GetInterferenceReason(cells[i], cells[j]);
                if (reason is null) continue;

                var overlap = _coverageAnalyzer.Analyze(cells[i], cells[j]);
                if (!overlap.Overlaps) continue;

                findings.Add(new InterferenceFinding(
                    cells[i].Id, cells[j].Id, reason, overlap.DistanceMeters, overlap.OverlapMeters));
            }
        }

        return findings;
    }

    private static string? GetInterferenceReason(Cell a, Cell b) => (a, b) switch
    {
        (LteCell x, LteCell y) when x.Earfcn == y.Earfcn && x.PhysicalCellId == y.PhysicalCellId
            => "LTE PCI collision on same EARFCN",
        (LteCell x, LteCell y) when x.Earfcn == y.Earfcn
            => "LTE co-channel coverage overlap",
        (FiveGCell x, FiveGCell y) when x.Nrarfcn == y.Nrarfcn && x.PhysicalCellId == y.PhysicalCellId
            => "5G NR PCI collision on same NR-ARFCN",
        (FiveGCell x, FiveGCell y) when x.Nrarfcn == y.Nrarfcn
            => "5G NR co-channel coverage overlap",
        _ => null
    };
}
