using FluxoCaixa.Consolidado.Domain.Repositories;
using MediatR;

namespace FluxoCaixa.Consolidado.Application.Queries;

public sealed class ObterSaldoDiarioQueryHandler
    : IRequestHandler<ObterSaldoDiarioQuery, SaldoDiarioResponse?>
{
    private readonly ISaldoDiarioRepository _repository;

    public ObterSaldoDiarioQueryHandler(
        ISaldoDiarioRepository repository)
    {
        _repository = repository;
    }

    public async Task<SaldoDiarioResponse?> Handle(
        ObterSaldoDiarioQuery request,
        CancellationToken cancellationToken)
    {
        var saldo = await _repository.GetAsync(
            request.UserId,
            request.Data,
            cancellationToken);

        if (saldo is null)
            return null;

        return new SaldoDiarioResponse(
            saldo.Data,
            saldo.TotalCredito,
            saldo.TotalDebito,
            saldo.Saldo);
    }
}