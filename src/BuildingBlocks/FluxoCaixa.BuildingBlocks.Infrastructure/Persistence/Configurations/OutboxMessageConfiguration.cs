using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using FluxoCaixa.BuildingBlocks.Infrastructure.OutBox;

namespace FluxoCaixa.BuildingBlocks.Infrastructure.Persistence;

public sealed class OutboxMessageConfiguration 
    : IEntityTypeConfiguration<OutboxMessage>
{
    public void Configure(EntityTypeBuilder<OutboxMessage> builder)
    {
        builder.ToTable("OutboxMessages");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Type)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(x => x.Content)
            .IsRequired();

        builder.Property(x => x.OccurredOn)
            .IsRequired();

        builder.Property(x => x.ProcessedOn);

        builder.HasIndex(x => x.ProcessedOn);

        builder.HasIndex(x => x.OccurredOn);
    }
}