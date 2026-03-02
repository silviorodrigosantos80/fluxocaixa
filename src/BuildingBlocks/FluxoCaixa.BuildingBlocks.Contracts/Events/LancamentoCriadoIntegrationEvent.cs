using FluxoCaixa.BuildingBlocks.Domain.Enums;

namespace FluxoCaixa.BuildingBlocks.Contracts.Events;

public sealed class LancamentoCriadoIntegrationEvent
{
    public Guid EventId { get; }
    public Guid UserId { get; }
    public Guid LancamentoId { get; }
    public DateTime Data { get; }
    public decimal Valor { get; }
    public TipoLancamento Tipo { get; }
    public DateTime OccurredOn { get; }

    public LancamentoCriadoIntegrationEvent(
        Guid eventId,
        Guid userId,
        Guid lancamentoId,
        DateTime data,
        decimal valor,
        TipoLancamento tipo,
        DateTime occurredOn)
    {
        EventId = eventId;
        UserId = userId;
        LancamentoId = lancamentoId;
        Data = data;
        Valor = valor;
        Tipo = tipo;
        OccurredOn = occurredOn;
    }
}