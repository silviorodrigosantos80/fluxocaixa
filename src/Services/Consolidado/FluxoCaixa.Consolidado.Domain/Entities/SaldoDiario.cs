using FluxoCaixa.BuildingBlocks.Domain.Common;

namespace FluxoCaixa.Consolidado.Domain.Entities;

public sealed class SaldoDiario : AggregateRoot
{
    public Guid UserId { get; private set; }
    public DateOnly Data { get; private set; }

    public decimal TotalCredito { get; private set; }
    public decimal TotalDebito { get; private set; }
    public decimal Saldo { get; private set; }

    private SaldoDiario() { }

    public SaldoDiario(Guid userId, DateOnly data)
    {
        UserId = userId;
        Data = data;
        TotalCredito = 0;
        TotalDebito = 0;
        Saldo = 0;
    }

    public void AplicarCredito(decimal valor)
    {
        if (valor <= 0)
            throw new ArgumentException("Valor inválido.");

        TotalCredito += valor;
        Saldo += valor;
    }

    public void AplicarDebito(decimal valor)
    {
        if (valor <= 0)
            throw new ArgumentException("Valor inválido.");

        TotalDebito += valor;
        Saldo -= valor;
    }
}