using System.Text.Json;
using FluxoCaixa.BuildingBlocks.Domain.Common;
using FluxoCaixa.BuildingBlocks.Infrastructure.OutBox;
using FluxoCaixa.BuildingBlocks.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FluxoCaixa.Lancamentos.Infrastructure.Persistence;

public sealed class LancamentosDbContext : BaseDbContext
{
    public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();
    public LancamentosDbContext(DbContextOptions<LancamentosDbContext> options)
        : base(options)
    {
    }

    public override async Task<int> SaveChangesAsync(
        CancellationToken cancellationToken = default)
    {
        AddOutboxMessages();

        return await base.SaveChangesAsync(cancellationToken);
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(LancamentosDbContext).Assembly);
    }

    private void AddOutboxMessages()
    {
        var domainEvents = ChangeTracker
            .Entries<AggregateRoot>()
            .SelectMany(entry => entry.Entity.DomainEvents)
            .ToList();

        foreach (var domainEvent in domainEvents)
        {
            var outboxMessage = new OutboxMessage
            (
                Guid.NewGuid(),
                domainEvent.OccurredOn,
                domainEvent.GetType().FullName!,
                JsonSerializer.Serialize(
                    domainEvent,
                    domainEvent.GetType())
            );

            OutboxMessages.Add(outboxMessage);
        }

        foreach (var entry in ChangeTracker.Entries<AggregateRoot>())
        {
            entry.Entity.ClearDomainEvents();
        }
    }
}