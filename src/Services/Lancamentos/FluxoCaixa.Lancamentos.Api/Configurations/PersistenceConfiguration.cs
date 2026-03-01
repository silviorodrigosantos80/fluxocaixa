using FluxoCaixa.Lancamentos.Domain.Abstractions;
using FluxoCaixa.Lancamentos.Infrastructure.Persistence;
using FluxoCaixa.BuildingBlocks.Infrastructure.Extensions;

namespace FluxoCaixa.Lancamentos.Api.Configurations;
public static class PersistenceConfiguration
{
    public static IServiceCollection AddPersistence(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbPostgres<LancamentosDbContext>(configuration, "LancamentosConnection");
        services.AddScoped<ILancamentoRepository, LancamentoRepository>();
        return services;
    }
}