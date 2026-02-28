namespace FluxoCaixa.BuildingBlocks.Infrastructure.Messaging;

public sealed class MassTransitOptions
{
    public const string SectionName = "MassTransit";

    public int PrefetchCount { get; init; } = 32;
    public int ConcurrentMessageLimit { get; init; } = 16;

    public RetryOptions Retry { get; init; } = new();
}

public sealed class RetryOptions
{
    public int RetryLimit { get; init; } = 5;
    public int MinIntervalMs { get; init; } = 200;
    public int MaxIntervalMs { get; init; } = 5000;
    public int IntervalDeltaMs { get; init; } = 500;
}