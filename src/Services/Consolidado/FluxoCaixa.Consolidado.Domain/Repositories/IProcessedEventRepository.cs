using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluxoCaixa.BuildingBlocks.Domain.Abstractions;
using FluxoCaixa.Consolidado.Domain.Entities;

namespace FluxoCaixa.Consolidado.Domain.Repositories
{
    public interface IProcessedEventRepository: IRepository<ProcessedEvent>
    {
        Task<bool> ExistsAsync(
            Guid eventId,
            CancellationToken cancellationToken);
    }
}