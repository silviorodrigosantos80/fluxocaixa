using FluxoCaixa.BuildingBlocks.Domain.Common;
using FluxoCaixa.BuildingBlocks.Domain.Exceptions;
using FluxoCaixa.BuildingBlocks.Domain.ValueObjects;

namespace FluxoCaixa.Lancamentos.Domain.ValueObjects;

public sealed class Valor : ValueObject
{
    public decimal Amount { get; }

    private Valor(decimal amount)
    {
        Amount = amount;
    }

    public static Valor Criar(decimal amount)
    {
        if (amount <= 0)
            throw new DomainException("Valor deve ser maior que zero.");

        return new Valor(amount);
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Amount;
    }
}