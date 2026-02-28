using FluxoCaixa.Consolidado.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FluxoCaixa.Consolidado.Infrastructure.Persistence.Configurations;

public sealed class SaldoDiarioConfiguration 
    : IEntityTypeConfiguration<SaldoDiario>
{
    public void Configure(EntityTypeBuilder<SaldoDiario> builder)
    {
        builder.ToTable("SaldosDiarios");

        builder.HasKey(x => new { x.UserId, x.Data });

        builder.Property(x => x.UserId)
            .IsRequired();

        builder.Property(x => x.Data)
            .IsRequired();

        builder.Property(x => x.TotalCredito)
            .HasColumnType("numeric(18,2)")
            .IsRequired();

        builder.Property(x => x.TotalDebito)
            .HasColumnType("numeric(18,2)")
            .IsRequired();

        builder.Property(x => x.Saldo)
            .HasColumnType("numeric(18,2)")
            .IsRequired();

        builder.HasIndex(x => new { x.UserId, x.Data });
    }
}