using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluxoCaixa.BuildingBlocks.Domain.Abstractions;
using FluxoCaixa.Consolidado.Domain.Entities;

namespace FluxoCaixa.Consolidado.Domain.Repositories;
public interface ISaldoDiarioRepository: IRepository<SaldoDiario>
{
    Task<SaldoDiario?> GetAsync(
        Guid userId,
        DateOnly data,
        CancellationToken cancellationToken);
}