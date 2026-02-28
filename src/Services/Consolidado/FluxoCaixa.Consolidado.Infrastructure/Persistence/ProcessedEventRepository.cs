using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluxoCaixa.BuildingBlocks.Infrastructure.Persistence;
using FluxoCaixa.Consolidado.Domain.Entities;
using FluxoCaixa.Consolidado.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace FluxoCaixa.Consolidado.Infrastructure.Persistence;
public sealed class ProcessedEventRepository 
    : GenericRepository<ProcessedEvent>, IProcessedEventRepository
{
    public ProcessedEventRepository(ConsolidadoDbContext context)
        : base(context)
    {
    }

    public async Task<bool> ExistsAsync(
        Guid eventId,
        CancellationToken cancellationToken)
    {
        return await this.DbSet
            .AnyAsync(x => x.EventId == eventId, cancellationToken);
    }

}