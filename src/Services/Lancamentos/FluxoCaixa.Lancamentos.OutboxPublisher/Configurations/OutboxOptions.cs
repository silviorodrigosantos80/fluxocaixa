using System.ComponentModel.DataAnnotations;

namespace FluxoCaixa.Lancamentos.OutboxPublisher.Configurations;

public sealed class OutboxOptions
{
    public const string SectionName = "Outbox";

    [Range(1, 1000)]
    public int BatchSize { get; init; }

    [Range(100, 10000)]
    public int DelayMilliseconds { get; init; }

    [Range(1, 64)]
    public int MaxParallelism { get; init; }
    [Range(1, 20)]
    public int MaxRetry { get; init; } = 5;
}