using InterferenceDetection.Application.Abstractions;
using InterferenceDetection.Application.Services;
using InterferenceDetection.Infrastructure.Persistence;
using InterferenceDetection.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace InterferenceDetection.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Postgres")
            ?? throw new InvalidOperationException("Missing connection string 'Postgres'.");

        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(connectionString).UseSnakeCaseNamingConvention());

        services.AddScoped<ICellRepository, CellRepository>();
        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<AppDbContext>());

        services.AddSingleton<IDistanceCalculator, HaversineDistanceCalculator>();
        services.AddSingleton<ICoverageAnalyzer, CircularCoverageAnalyzer>();
        services.AddSingleton<IInterferenceDetector, FrequencyInterferenceDetector>();

        return services;
    }
}
