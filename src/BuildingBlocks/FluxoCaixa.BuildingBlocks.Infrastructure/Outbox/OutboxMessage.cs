namespace FluxoCaixa.BuildingBlocks.Infrastructure.OutBox;

public sealed class OutboxMessage
{
    public Guid Id { get; private set; }
    public DateTime OccurredOn { get; private set; }
    public string Type { get; private set; }
    public string Content { get; private set; }
    public DateTime? ProcessedOn { get; private set; }
    public int RetryCount { get; private set; }
    public DateTime? LastAttemptOn { get; private set; }
    public string? LastError { get; private set; }
    public bool IsDeadLetter { get; private set; }

    private const int MaxRetryDefault = 5; 

    private OutboxMessage() { } // EF

    public OutboxMessage(
        Guid id,
        DateTime occurredOn,
        string type,
        string content)
    {
        Id = id;
        OccurredOn = occurredOn;
        Type = type ?? throw new ArgumentNullException(nameof(type));
        Content = content ?? throw new ArgumentNullException(nameof(content));
    }

    public void MarkAsProcessed()
    {
        if (ProcessedOn.HasValue)
            return; //evitar reprocessamento duplicado

        ProcessedOn = DateTime.UtcNow;
    }

    public void RegisterFailure(string error, int maxRetry)
    {
        if (IsProcessed())
            return;

        RetryCount++;
        LastAttemptOn = DateTime.UtcNow;
        LastError = error;

        if (RetryCount >= maxRetry)
            IsDeadLetter = true;
    }

    public bool IsProcessed() => ProcessedOn.HasValue;

    public bool CanBeRetried(int? maxRetry = null)
    {
        if (IsProcessed())
            return false;

        var max = maxRetry ?? MaxRetryDefault;

        return !IsDeadLetter && RetryCount < max;
    }
}