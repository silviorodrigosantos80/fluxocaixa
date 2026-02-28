using FluxoCaixa.BuildingBlocks.Domain.Common;
using FluxoCaixa.Lancamentos.Domain.Enums;
using FluxoCaixa.Lancamentos.Domain.Events;
using FluxoCaixa.Lancamentos.Domain.ValueObjects;

namespace FluxoCaixa.Lancamentos.Domain.Entities;

public sealed class Lancamento : AggregateRoot
{
    public Guid UserId { get; private set; }
    public DateTime Data { get; private set; }
    public Valor Valor { get; private set; } = null!;
    public TipoLancamento Tipo { get; private set; }


    private Lancamento() { } // EF

    private Lancamento(Guid lancamentoId, Guid userId, DateTime data, Valor valor, TipoLancamento tipo)
        : base(lancamentoId)
    {
        Id = lancamentoId;
        UserId = userId;
        Data = data;
        Valor = valor;
        Tipo = tipo;

        AddDomainEvent(
            new LancamentoCriadoDomainEvent(
                Guid.NewGuid(),
                UserId,
                Id,
                Data,
                Valor.Amount,
                (int)Tipo));
    }

    public static Lancamento Criar(Guid userId, DateTime data, decimal valor, TipoLancamento tipo)
    {
        var valorVO = Valor.Criar(valor);

        return new Lancamento(
            Guid.NewGuid(),
            userId, 
            data,
            valorVO,
            tipo);
    }
}