namespace FluxoCaixa.BuildingBlocks.Infrastructure.OutBox;

public sealed class OutboxMessage
{
    public Guid Id { get; private set; }
    public DateTime OccurredOn { get; private set; }
    public string Type { get; private set; }
    public string Content { get; private set; }
    public DateTime? ProcessedOn { get; private set; }

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
            return; // idempotente

        ProcessedOn = DateTime.UtcNow;
    }

    public bool IsProcessed() => ProcessedOn.HasValue;
}