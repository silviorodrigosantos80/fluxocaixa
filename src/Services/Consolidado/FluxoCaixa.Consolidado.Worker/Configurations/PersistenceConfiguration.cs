using FluxoCaixa.Consolidado.Domain.Repositories;
using FluxoCaixa.Consolidado.Infrastructure.Persistence;
using FluxoCaixa.BuildingBlocks.Infrastructure.Extensions;

namespace FluxoCaixa.Consolidado.Worker.Configurations;
public static class PersistenceConfiguration
{
    public static IServiceCollection AddPersistence(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbPostgres<ConsolidadoDbContext>(configuration, "ConsolidadoConnection");
        
        services.AddScoped<ISaldoDiarioRepository, SaldoDiarioRepository>();
        services.AddScoped<IProcessedEventRepository, ProcessedEventRepository>();

        return services;
    }
}