namespace InfraStellar.Application.DTOs;

/// <summary>
/// Resultado estruturado de um scraping de publicação.
/// Representa a publicação mais recente encontrada na página monitorada.
/// </summary>
public class PublicacaoScrapingResult
{
    /// <summary>Título do documento/publicação (ex: "PUBLICAÇÃO DA HOMOLOGAÇÃO DO SELETIVO").</summary>
    public string Titulo { get; set; } = string.Empty;

    /// <summary>Data textual extraída da página (ex: "02/06/2026").</summary>
    public string Data { get; set; } = string.Empty;

    /// <summary>URL absoluta do edital/documento para download, quando disponível.</summary>
    public string? Link { get; set; }
}
