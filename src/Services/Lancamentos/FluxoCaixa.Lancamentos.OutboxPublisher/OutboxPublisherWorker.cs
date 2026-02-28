using System.Text.Json;
using FluxoCaixa.BuildingBlocks.Contracts.Events;
using FluxoCaixa.Lancamentos.Domain.Events;
using FluxoCaixa.Lancamentos.Infrastructure.Persistence;
using FluxoCaixa.Lancamentos.OutboxPublisher.Configurations;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

public sealed class OutboxPublisherWorker : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<OutboxPublisherWorker> _logger;
    private readonly OutboxOptions _outboxOptions;

    public OutboxPublisherWorker(
        IServiceProvider serviceProvider,
        IOptions<OutboxOptions> outboxOptions,
        IConfiguration configuration,
        ILogger<OutboxPublisherWorker> logger)
    {
        _serviceProvider = serviceProvider;
        _outboxOptions = outboxOptions.Value;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Outbox Publisher Worker iniciado.");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();

                var dbContext = scope.ServiceProvider
                    .GetRequiredService<LancamentosDbContext>();

                var publishEndpoint = scope.ServiceProvider
                    .GetRequiredService<IPublishEndpoint>();

                var messages = await dbContext.OutboxMessages
                    .FromSqlRaw(@"
                        SELECT * FROM ""OutboxMessages""
                        WHERE ""ProcessedOn"" IS NULL
                        AND ""IsDeadLetter"" = false
                        ORDER BY ""OccurredOn""
                        FOR UPDATE SKIP LOCKED
                        LIMIT {0}", _outboxOptions.BatchSize)
                    .ToListAsync(stoppingToken);

                if (messages.Count == 0)
                {
                    await Task.Delay(_outboxOptions.DelayMilliseconds, stoppingToken);
                    continue;
                }

                _logger.LogInformation("Processando {Count} mensagens da Outbox.", messages.Count);

                await Parallel.ForEachAsync(
                    messages,
                    new ParallelOptions
                    {
                        MaxDegreeOfParallelism = _outboxOptions.MaxParallelism,
                        CancellationToken = stoppingToken
                    },
                    async (message, token) =>
                    {
                        try
                        {
                            if (message.Type ==
                                typeof(LancamentoCriadoIntegrationEvent).FullName)
                            {
                                var domainEvent =
                                    JsonSerializer.Deserialize<LancamentoCriadoDomainEvent>(
                                        message.Content);

                                if (domainEvent is null)
                                {
                                    _logger.LogWarning(
                                        "Falha ao desserializar evento: {Id}", 
                                        message.Id);
                                    return;
                                }

                                var integrationEvent =
                                    new LancamentoCriadoIntegrationEvent(
                                        domainEvent.EventId,
                                        domainEvent.UserId,
                                        domainEvent.LancamentoId,
                                        domainEvent.Data,
                                        domainEvent.Valor,
                                        domainEvent.Tipo,
                                        domainEvent.OccurredOn);

                                await publishEndpoint.Publish(
                                    integrationEvent,
                                    token);
                            }
                            message.MarkAsProcessed();
                        }
                        catch (Exception ex)
                        {
                            message.RegisterFailure(ex.Message, _outboxOptions.MaxRetry);
                            _logger.LogError(
                                ex,
                                "Erro ao publicar mensagem {Id}",
                                message.Id);
                        }
                    });

                await dbContext.SaveChangesAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro inesperado no OutboxPublisherWorker.");
            }

            await Task.Delay(_outboxOptions.DelayMilliseconds, stoppingToken);
        }

        _logger.LogInformation("Outbox Publisher Worker finalizado.");
    }
}