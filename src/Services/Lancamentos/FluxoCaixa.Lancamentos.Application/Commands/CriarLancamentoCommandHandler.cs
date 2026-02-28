using FluxoCaixa.BuildingBlocks.Application.Abstractions;
using FluxoCaixa.BuildingBlocks.Application.Results;
using FluxoCaixa.Lancamentos.Domain.Abstractions;
using FluxoCaixa.Lancamentos.Domain.Entities;

namespace FluxoCaixa.Lancamentos.Application.Commands.CriarLancamento;

public sealed class CriarLancamentoCommandHandler
    : ICommandHandler<CriarLancamentoCommand, Result<Guid>>
{
    private readonly ILancamentoRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public CriarLancamentoCommandHandler(
        ILancamentoRepository repository,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(
        CriarLancamentoCommand request,
        CancellationToken cancellationToken)
    {
        var lancamento = Lancamento.Criar(
            request.UserId,
            request.Data,
            request.Valor,
            request.Tipo);

        await _repository.AddAsync(lancamento, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<Guid>.Success(lancamento.Id);
    }
}