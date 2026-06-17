using InfraStellar.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InfraStellar.Infrastructure.Data.Configurations;

public class ResultadoAlertaConfiguration : IEntityTypeConfiguration<ResultadoAlerta>
{
    public void Configure(EntityTypeBuilder<ResultadoAlerta> builder)
    {
        builder.HasKey(r => r.Id);

        builder.Property(r => r.TituloPublicacao)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(r => r.DataPublicacao)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(r => r.LinkPublicacao)
            .HasMaxLength(2000);

        builder.Property(r => r.MensagemErro)
            .HasMaxLength(2000);

        builder.Property(r => r.ExecutadoEm)
            .HasDefaultValueSql("SYSDATETIMEOFFSET()");

        // Relacionamento: ResultadoAlerta pertence a Alerta
        builder.HasOne(r => r.Alerta)
            .WithMany(a => a.Resultados)
            .HasForeignKey(r => r.AlertaId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(r => new { r.AlertaId, r.ExecutadoEm });
    }
}
