using FluxoCaixa.Lancamentos.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FluxoCaixa.Lancamentos.Infrastructure.Persistence.Configurations;

public sealed class LancamentoConfiguration 
    : IEntityTypeConfiguration<Lancamento>
{
    public void Configure(EntityTypeBuilder<Lancamento> builder)
    {
        builder.ToTable("Lancamentos");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.UserId)
            .IsRequired();

        builder.HasIndex(x => x.UserId);

        builder.HasIndex(x => new { x.UserId, x.Data });

        builder.Property(x => x.Data)
            .IsRequired();

        builder.Property(x => x.Tipo)
            .HasConversion<int>()
            .IsRequired();

        builder.OwnsOne(x => x.Valor, valor =>
        {
            valor.Property(v => v.Amount)
                .HasColumnName("Valor")
                .HasColumnType("numeric(18,2)")
                .IsRequired();
        });
    }
}