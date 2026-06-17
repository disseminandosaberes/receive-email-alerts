namespace InfraStellar.Domain.Entities;

/// <summary>
/// Registra o resultado de uma execução de monitoramento para um alerta.
/// Cada execução (manual ou automática) gera um ResultadoAlerta com os dados
/// da publicação mais recente encontrada e se ela é nova em relação à anterior.
/// </summary>
public class ResultadoAlerta
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid AlertaId { get; set; }

    /// <summary>Título do documento/publicação mais recente encontrado na lista.</summary>
    public string TituloPublicacao { get; set; } = string.Empty;

    /// <summary>Data textual do documento encontrado (ex: "02/06/2026").</summary>
    public string DataPublicacao { get; set; } = string.Empty;

    /// <summary>Link (href) do documento encontrado, quando disponível.</summary>
    public string? LinkPublicacao { get; set; }

    /// <summary>
    /// True se o título encontrado é diferente do último título conhecido —
    /// ou seja, uma nova publicação foi detectada desde o último scraping.
    /// </summary>
    public bool NovaPublicacaoDetectada { get; set; }

    public DateTimeOffset ExecutadoEm { get; set; } = DateTimeOffset.UtcNow;

    /// <summary>True se o scraping completou sem erros técnicos.</summary>
    public bool Sucesso { get; set; }

    /// <summary>Mensagem de erro técnico, se houver.</summary>
    public string? MensagemErro { get; set; }

    // --- Navigation ---
    public Alerta? Alerta { get; set; }
}
