using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluxoCaixa.BuildingBlocks.Infrastructure.Persistence;
using FluxoCaixa.Consolidado.Domain.Entities;
using FluxoCaixa.Consolidado.Domain.Repositories;

namespace FluxoCaixa.Consolidado.Infrastructure.Persistence;

public sealed class SaldoDiarioRepository 
    : GenericRepository<SaldoDiario>, ISaldoDiarioRepository
{
    public SaldoDiarioRepository(ConsolidadoDbContext context)
        : base(context)
    {
    }

    public async Task<SaldoDiario?> GetAsync(
        Guid userId,
        DateOnly data,
        CancellationToken cancellationToken)
    {
        return await this.DbSet.FindAsync(new { userId, data });
    }

}