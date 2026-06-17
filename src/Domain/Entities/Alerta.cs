using InfraStellar.Domain.Enums;

namespace InfraStellar.Domain.Entities;

/// <summary>
/// Alerta de monitoramento de publicações criado por um usuário.
/// Cada alerta acompanha uma página específica de um site mapeado (TipoAlerta),
/// detectando quando uma nova publicação é adicionada à lista monitorada.
/// </summary>
public class Alerta
{
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>Usuário dono deste alerta.</summary>
    public Guid UsuarioId { get; set; }

    /// <summary>Nome amigável dado pelo usuário (ex: "Residência Técnica SEPLAG 2026").</summary>
    public string Nome { get; set; } = string.Empty;

    /// <summary>
    /// Tipo/domínio do alerta. Define qual estratégia de scraping será usada
    /// e qual é a estrutura HTML esperada na página.
    /// </summary>
    public TipoAlerta TipoAlerta { get; set; }

    /// <summary>
    /// URL específica da página a ser monitorada.
    /// Ex: https://seletivo.seplag.mt.gov.br/detalhes/51?slug=...
    /// </summary>
    public string Url { get; set; } = string.Empty;

    /// <summary>Se falso, o alerta não será processado pelo background service.</summary>
    public bool Ativo { get; set; } = true;

    public DateTimeOffset CriadoEm { get; set; } = DateTimeOffset.UtcNow;

    // --- Estado do monitoramento ---

    /// <summary>
    /// Título da última publicação já conhecida e processada.
    /// Usado para comparar com a publicação mais recente encontrada no próximo scraping.
    /// Null = nunca executado (primeiro scraping apenas registra sem gerar notificação).
    /// </summary>
    public string? UltimoTituloConhecido { get; set; }

    /// <summary>Data textual da última publicação conhecida (ex: "02/06/2026").</summary>
    public string? UltimaDataConhecida { get; set; }

    /// <summary>Frequência (em minutos) em que este alerta deve ser verificado em background.</summary>
    public int IntervaloMinutos { get; set; } = 60;

    /// <summary>Data e hora em que este alerta foi processado pela última vez (manual ou automático).</summary>
    public DateTimeOffset? UltimaExecucaoEm { get; set; }

    // --- Navigation Properties ---
    public Usuario? Usuario { get; set; }
    public ICollection<ResultadoAlerta> Resultados { get; set; } = new List<ResultadoAlerta>();
}
