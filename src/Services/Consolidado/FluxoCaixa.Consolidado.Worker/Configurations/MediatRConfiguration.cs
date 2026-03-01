using FluxoCaixa.Consolidado.Application.Commands;
using MediatR;

namespace FluxoCaixa.Consolidado.Worker.Configurations;

public static class MediatRConfiguration
{
    public static IServiceCollection AddApplication(
        this IServiceCollection services)
    {
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(
                typeof(ProcessarLancamentoIntegrationCommand).Assembly);
        });

        return services;
    }
}