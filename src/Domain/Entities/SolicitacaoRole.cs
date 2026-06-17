using System;

namespace InfraStellar.Domain.Entities;

public class SolicitacaoRole
{
    public Guid Id { get; set; }
    public Guid UsuarioId { get; set; }
    public Usuario? Usuario { get; set; }
    public string RoleNome { get; set; } = string.Empty;
    public string Status { get; set; } = "Pendente"; // Pendente, Aprovada, Rejeitada
    public DateTime DataSolicitacao { get; set; } = DateTime.UtcNow;
    public DateTime? DataProcessamento { get; set; }
    public string? ProcessadoPor { get; set; } // E-mail ou username do admin que aprovou/rejeitou
}
