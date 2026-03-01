using FluxoCaixa.Lancamentos.Infrastructure.Persistence;
using FluxoCaixa.BuildingBlocks.Infrastructure.Extensions;

namespace FluxoCaixa.Lancamentos.OutboxPublisher.Configurations
{
    public static class PersistenceConfiguration
    {
        public static IServiceCollection AddPersistence(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddDbPostgres<LancamentosDbContext>(configuration, "LancamentosConnection");

            return services;
        }
    }
}