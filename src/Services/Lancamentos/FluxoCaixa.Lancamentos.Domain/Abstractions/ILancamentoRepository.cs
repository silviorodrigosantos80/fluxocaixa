
using FluxoCaixa.BuildingBlocks.Domain.Abstractions;
using FluxoCaixa.Lancamentos.Domain.Entities;

namespace FluxoCaixa.Lancamentos.Domain.Abstractions;

public interface ILancamentoRepository : IRepository<Lancamento>
{
}