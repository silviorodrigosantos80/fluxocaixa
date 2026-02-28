namespace FluxoCaixa.Consolidado.Application.Queries;

public sealed record SaldoDiarioResponse(
    DateOnly Data,
    decimal TotalCredito,
    decimal TotalDebito,
    decimal Saldo
);