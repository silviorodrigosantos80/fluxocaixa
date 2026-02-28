namespace FluxoCaixa.BuildingBlocks.Domain.Events;

public interface IDomainEvent
{
    DateTime OccurredOn { get; }
}