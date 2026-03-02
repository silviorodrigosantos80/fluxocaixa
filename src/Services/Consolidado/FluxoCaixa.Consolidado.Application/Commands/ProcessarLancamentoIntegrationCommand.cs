using FluxoCaixa.BuildingBlocks.Domain.Enums;
using MediatR;

namespace FluxoCaixa.Consolidado.Application.Commands;

public sealed record ProcessarLancamentoIntegrationCommand(
    Guid EventId,
    Guid UserId,
    DateTime Data,
    decimal Valor,
    TipoLancamento Tipo
) : IRequest;