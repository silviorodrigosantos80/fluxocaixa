using FluxoCaixa.Consolidado.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FluxoCaixa.Consolidado.Infrastructure.Persistence.Configurations;

public sealed class ProcessedEventConfiguration 
    : IEntityTypeConfiguration<ProcessedEvent>
{
    public void Configure(EntityTypeBuilder<ProcessedEvent> builder)
    {
        builder.ToTable("ProcessedEvents");

        builder.HasKey(x => x.EventId);

        builder.Property(x => x.ProcessedOn)
            .IsRequired();
    }
}