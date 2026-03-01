using FluxoCaixa.Lancamentos.Application.Commands.CriarLancamento;

namespace FluxoCaixa.Lancamentos.Api.Configurations;
public static class MediatRConfiguration
{
    public static IServiceCollection AddApplication(
        this IServiceCollection services)
    {
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(
                typeof(CriarLancamentoCommandHandler).Assembly);
        });

        return services;
    }
}