using FluxoCaixa.BuildingBlocks.Infrastructure.Persistence;
using FluxoCaixa.Lancamentos.Domain.Abstractions;
using FluxoCaixa.Lancamentos.Domain.Entities;

namespace FluxoCaixa.Lancamentos.Infrastructure.Persistence;

public sealed class LancamentoRepository 
    : GenericRepository<Lancamento>, ILancamentoRepository
{
    public LancamentoRepository(LancamentosDbContext context)
        : base(context)
    {
    }
}