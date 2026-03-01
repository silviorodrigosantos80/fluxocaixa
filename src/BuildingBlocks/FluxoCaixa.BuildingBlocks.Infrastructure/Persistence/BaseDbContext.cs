using System.Text.Json;
using FluxoCaixa.BuildingBlocks.Domain.Common;
using FluxoCaixa.BuildingBlocks.Infrastructure.OutBox;
using Microsoft.EntityFrameworkCore;

namespace FluxoCaixa.BuildingBlocks.Infrastructure.Persistence;

public abstract class BaseDbContext : DbContext
{
    protected BaseDbContext(DbContextOptions options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(BaseDbContext).Assembly);

        base.OnModelCreating(modelBuilder);
    }

    public override async Task<int> SaveChangesAsync(
        CancellationToken cancellationToken = default)
    {
        return await base.SaveChangesAsync(cancellationToken);
    }
}