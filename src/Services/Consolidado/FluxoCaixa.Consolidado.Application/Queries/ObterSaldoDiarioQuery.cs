using MediatR;

namespace FluxoCaixa.Consolidado.Application.Queries;

public sealed record ObterSaldoDiarioQuery(
    Guid UserId,
    DateOnly Data
) : IRequest<SaldoDiarioResponse?>;