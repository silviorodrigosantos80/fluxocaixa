using FluxoCaixa.BuildingBlocks.Domain.Common;
using FluxoCaixa.BuildingBlocks.Domain.Events;

namespace FluxoCaixa.Lancamentos.Domain.Events;

public sealed class LancamentoCriadoDomainEvent : IDomainEvent
{
    public Guid EventId { get; }
    public Guid UserId { get; }
    public Guid LancamentoId { get; }
    public DateTime Data { get; }
    public decimal Valor { get; }
    public int Tipo { get; }
    public DateTime OccurredOn { get; }

    public LancamentoCriadoDomainEvent(
        Guid eventId,
        Guid userId,
        Guid lancamentoId,
        DateTime data,
        decimal valor,
        int tipo)
    {
        EventId = eventId;
        UserId = userId;
        LancamentoId = lancamentoId;
        Data = data;
        Valor = valor;
        Tipo = tipo;
        OccurredOn = DateTime.UtcNow;
    }
}