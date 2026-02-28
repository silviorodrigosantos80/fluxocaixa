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
    
    public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();

    public override async Task<int> SaveChangesAsync(
        CancellationToken cancellationToken = default)
    {
        AddOutboxMessages();

        return await base.SaveChangesAsync(cancellationToken);
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
                domainEvent.GetType().AssemblyQualifiedName!,
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