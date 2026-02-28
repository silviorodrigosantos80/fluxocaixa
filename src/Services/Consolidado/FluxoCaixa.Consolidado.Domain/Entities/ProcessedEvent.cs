namespace FluxoCaixa.Consolidado.Domain.Entities;

public sealed class ProcessedEvent
{
    public Guid EventId { get; private set; }
    public DateTime ProcessedOn { get; private set; }

    private ProcessedEvent() { }

    public ProcessedEvent(Guid eventId)
    {
        EventId = eventId;
        ProcessedOn = DateTime.UtcNow;
    }
}