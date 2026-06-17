using InfraStellar.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InfraStellar.Infrastructure.Data.Configurations;

public class AlertaConfiguration : IEntityTypeConfiguration<Alerta>
{
    public void Configure(EntityTypeBuilder<Alerta> builder)
    {
        builder.HasKey(a => a.Id);

        builder.Property(a => a.Nome)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(a => a.Url)
            .IsRequired()
            .HasMaxLength(2000);

        builder.Property(a => a.TipoAlerta)
            .HasConversion<int>();

        builder.Property(a => a.UltimoTituloConhecido)
            .HasMaxLength(500);

        builder.Property(a => a.UltimaDataConhecida)
            .HasMaxLength(50);

        builder.Property(a => a.IntervaloMinutos)
            .IsRequired()
            .HasDefaultValue(60);

        builder.Property(a => a.UltimaExecucaoEm);

        builder.Property(a => a.CriadoEm)
            .HasDefaultValueSql("SYSDATETIMEOFFSET()");

        // Relacionamento: Alerta pertence a Usuario
        builder.HasOne(a => a.Usuario)
            .WithMany()
            .HasForeignKey(a => a.UsuarioId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(a => a.UsuarioId);
        builder.HasIndex(a => new { a.UsuarioId, a.Ativo });
    }
}
