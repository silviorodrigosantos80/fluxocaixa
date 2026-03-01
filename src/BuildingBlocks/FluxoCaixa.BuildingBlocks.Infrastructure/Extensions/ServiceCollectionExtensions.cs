using FluxoCaixa.BuildingBlocks.Application.Abstractions;
using FluxoCaixa.BuildingBlocks.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FluxoCaixa.BuildingBlocks.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDbPostgres<TContext>(
        this IServiceCollection services,
        IConfiguration configuration, string connectionName)
        where TContext : BaseDbContext
    {
        services.AddDbContext<TContext>(options =>
        {
            options.UseNpgsql(
                configuration.GetConnectionString(connectionName));
        });

        services.AddScoped<IUnitOfWork, UnitOfWork<TContext>>();

        return services;
    }
}