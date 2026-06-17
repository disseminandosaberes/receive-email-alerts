using InfraStellar.Application.DTOs;
using InfraStellar.Domain.Enums;

namespace InfraStellar.Application.Interfaces;

/// <summary>
/// Serviço de scraping headless especializado por domínio.
/// Para cada TipoAlerta, conhece a estrutura HTML específica do site
/// e extrai a publicação mais recente de forma estruturada.
/// </summary>
public interface IScrapingService
{
    /// <summary>
    /// Acessa a URL informada e extrai a publicação mais recente,
    /// usando a estratégia de scraping correspondente ao <paramref name="tipo"/>.
    /// </summary>
    /// <param name="url">URL específica da página a monitorar.</param>
    /// <param name="tipo">Tipo do alerta — determina qual seletor e lógica usar.</param>
    /// <returns>Dados estruturados da publicação mais recente encontrada.</returns>
    /// <exception cref="InvalidOperationException">Quando nenhuma publicação é encontrada na página.</exception>
    Task<PublicacaoScrapingResult> ExtrairPublicacaoMaisRecenteAsync(string url, TipoAlerta tipo);
}
