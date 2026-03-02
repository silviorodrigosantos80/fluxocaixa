using FluxoCaixa.BuildingBlocks.Infrastructure.Messaging;
using FluxoCaixa.Consolidado.Infrastructure.Messaging.Consumers;
using MassTransit;
using Microsoft.Extensions.Options;

namespace FluxoCaixa.Consolidado.Worker.Configurations;

public static class MessagingConfiguration
{
    public static IServiceCollection AddMessaging(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        
        services
            .AddOptions<RabbitMqOptions>()
            .Bind(configuration.GetSection(RabbitMqOptions.SectionName))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services
        .AddOptions<MassTransitOptions>()
        .Bind(configuration.GetSection(MassTransitOptions.SectionName))
        .ValidateOnStart();
        
        services.AddMassTransit(x =>
        {
            x.AddConsumer<LancamentoCriadoConsumer>();
            
            x.UsingRabbitMq((context, cfg) =>
            {
                var rabbitOptions = context
                    .GetRequiredService<IOptions<RabbitMqOptions>>()
                    .Value;

                var mtOptions = context
                    .GetRequiredService<IOptions<MassTransitOptions>>()
                    .Value;

                cfg.Host(rabbitOptions.Host, h =>
                {
                    h.Username(rabbitOptions.Username);
                    h.Password(rabbitOptions.Password);
                });

                cfg.ReceiveEndpoint("consolidado-lancamento-criado", e =>
                {
                    e.PrefetchCount = (ushort)mtOptions.PrefetchCount;

                    e.UseMessageRetry(r =>
                    {
                        r.Exponential(
                            retryLimit: mtOptions.Retry.RetryLimit,
                            minInterval: TimeSpan.FromMilliseconds(mtOptions.Retry.MinIntervalMs),
                            maxInterval: TimeSpan.FromMilliseconds(mtOptions.Retry.MaxIntervalMs),
                            intervalDelta: TimeSpan.FromMilliseconds(mtOptions.Retry.IntervalDeltaMs));
                    });

                    e.ConcurrentMessageLimit = mtOptions.ConcurrentMessageLimit;

                    e.ConfigureConsumer<LancamentoCriadoConsumer>(context);
                });
            });
        });

        return services;
    }
}