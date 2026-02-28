using MediatR;

namespace FluxoCaixa.Consolidado.Api.Configurations;

public static class MediatRConfiguration
{
    public static IServiceCollection AddApplication(
        this IServiceCollection services)
    {
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(
                typeof(FluxoCaixa.Consolidado.Application.Commands.ProcessarLancamentoIntegrationCommand).Assembly);
        });

        return services;
    }
}