using MediatR;

namespace FluxoCaixa.Consolidado.Application.Commands;

public sealed record ProcessarLancamentoIntegrationCommand(
    Guid EventId,
    Guid UserId,
    DateTime Data,
    decimal Valor,
    int Tipo
) : IRequest;