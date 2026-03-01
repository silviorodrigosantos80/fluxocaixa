using FluxoCaixa.BuildingBlocks.Domain.Common;

namespace FluxoCaixa.Consolidado.Domain.Entities;

public sealed class ProcessedEvent: AggregateRoot
{
    public DateTime ProcessedOn { get; private set; }

    private ProcessedEvent() { }

    public ProcessedEvent(Guid eventId)
    {
        Id = eventId;
        ProcessedOn = DateTime.UtcNow;
    }
}