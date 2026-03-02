using FluxoCaixa.BuildingBlocks.Application.Abstractions;
using FluxoCaixa.BuildingBlocks.Domain.Enums;
using FluxoCaixa.Consolidado.Domain.Entities;
using FluxoCaixa.Consolidado.Domain.Repositories;
using MediatR;

namespace FluxoCaixa.Consolidado.Application.Commands;

public sealed class ProcessarLancamentoIntegrationCommandHandler
    : IRequestHandler<ProcessarLancamentoIntegrationCommand>
{
    private readonly ISaldoDiarioRepository _saldoRepository;
    private readonly IProcessedEventRepository _processedRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ProcessarLancamentoIntegrationCommandHandler(
        ISaldoDiarioRepository saldoRepository,
        IProcessedEventRepository processedRepository,
        IUnitOfWork unitOfWork)
    {
        _saldoRepository = saldoRepository;
        _processedRepository = processedRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(
        ProcessarLancamentoIntegrationCommand request,
        CancellationToken cancellationToken)
    {
        var alreadyProcessed =
            await _processedRepository.ExistsAsync(
                request.EventId,
                cancellationToken);

        if (alreadyProcessed)
            return;

        var data = DateOnly.FromDateTime(request.Data);

        var saldo = await _saldoRepository.GetAsync(
            request.UserId,
            data,
            cancellationToken);

        if (saldo is null)
        {
            saldo = new SaldoDiario(
                request.UserId,
                data);

            await _saldoRepository.AddAsync(
                saldo,
                cancellationToken);
        }

        switch(request.Tipo)
        {
            case  TipoLancamento.Credito:
                saldo.AplicarCredito(request.Valor);
            break;
            case  TipoLancamento.Debito:
                saldo.AplicarDebito(request.Valor);
            break;
            default:
                throw new Exception("Tipo de lançamento inválido!");
        }
    
        await _processedRepository.AddAsync(
            new ProcessedEvent(request.EventId),
            cancellationToken);

        await _unitOfWork
            .SaveChangesAsync(cancellationToken);
    }
    private void swith(TipoLancamento tipo)
    {
        throw new NotImplementedException();
    }
}