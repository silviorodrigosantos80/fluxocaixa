using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluxoCaixa.Consolidado.Application.Queries;

namespace FluxoCaixa.Consolidado.Api.Configurations;
public static class MediatRConfiguration
{
    public static IServiceCollection AddApplication(
        this IServiceCollection services)
    {
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(typeof(ObterSaldoDiarioQuery).Assembly);
        });

        return services;
    }
}