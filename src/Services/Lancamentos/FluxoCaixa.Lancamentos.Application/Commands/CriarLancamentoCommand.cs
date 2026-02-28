using FluxoCaixa.BuildingBlocks.Application.Abstractions;
using FluxoCaixa.BuildingBlocks.Application.Results;
using FluxoCaixa.Lancamentos.Domain.Enums;

namespace FluxoCaixa.Lancamentos.Application.Commands.CriarLancamento;

public sealed record CriarLancamentoCommand(
    Guid UserId,
    DateTime Data,
    decimal Valor,
    TipoLancamento Tipo
) : ICommand<Result<Guid>>;