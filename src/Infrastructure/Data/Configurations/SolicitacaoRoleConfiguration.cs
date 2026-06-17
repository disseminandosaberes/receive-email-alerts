using InfraStellar.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InfraStellar.Infrastructure.Data.Configurations;

public class SolicitacaoRoleConfiguration : IEntityTypeConfiguration<SolicitacaoRole>
{
    public void Configure(EntityTypeBuilder<SolicitacaoRole> builder)
    {
        builder.ToTable("SolicitacoesRole");

        builder.HasKey(s => s.Id);

        builder.Property(s => s.RoleNome)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(s => s.Status)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(s => s.ProcessadoPor)
            .HasMaxLength(150);

        builder.HasOne(s => s.Usuario)
            .WithMany()
            .HasForeignKey(s => s.UsuarioId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
