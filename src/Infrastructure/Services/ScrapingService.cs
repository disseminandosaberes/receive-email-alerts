using System;
using System.Net.Http;
using System.Threading.Tasks;
using HtmlAgilityPack;
using InfraStellar.Application.DTOs;
using InfraStellar.Application.Interfaces;
using InfraStellar.Domain.Enums;
using Microsoft.Extensions.Logging;

namespace InfraStellar.Infrastructure.Services;

/// <summary>
/// Implementação de <see cref="IScrapingService"/> usando HttpClient e HtmlAgilityPack.
/// Evita carregar instâncias pesadas de navegadores headless, consumindo muito menos recursos.
/// </summary>
public class ScrapingService : IScrapingService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<ScrapingService> _logger;

    public ScrapingService(IHttpClientFactory httpClientFactory, ILogger<ScrapingService> logger)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    public async Task<PublicacaoScrapingResult> ExtrairPublicacaoMaisRecenteAsync(string url, TipoAlerta tipo)
    {
        _logger.LogInformation("Iniciando scraping HttpClient [{Tipo}]: {Url}", tipo, url);

        // Limpa parâmetros de query (como ?slug=...) que acionam regras restritivas do WAF
        var cleanUrl = url;
        if (cleanUrl.Contains('?'))
        {
            cleanUrl = cleanUrl.Split('?')[0];
            _logger.LogInformation("URL normalizada para evitar WAF: {CleanUrl}", cleanUrl);
        }

        using var client = _httpClientFactory.CreateClient();
        
        // Define um User-Agent neutro para a requisição
        client.DefaultRequestHeaders.UserAgent.ParseAdd("HttpClient/10.0");

        var html = await client.GetStringAsync(cleanUrl);

        _logger.LogInformation("HTML baixado com sucesso. Tamanho: {Length}", html.Length);

        var doc = new HtmlDocument();
        doc.LoadHtml(html);

        return tipo switch
        {
            TipoAlerta.SeletivaSeplagMT => ExtrairSeletivaSeplagMT(doc, cleanUrl),
            _ => throw new NotSupportedException($"TipoAlerta '{tipo}' não possui estratégia de scraping implementada.")
        };
    }

    /// <summary>
    /// Estratégia para seletivo.seplag.mt.gov.br usando HtmlAgilityPack.
    ///
    /// Estrutura HTML:
    /// .detalhes-doc-list
    ///   └── a.detalhes-doc-card  (primeiro = mais recente)
    ///         ├── .doc-body
    ///         │     ├── span.doc-date   → "02/06/2026 · Download"
    ///         │     └── p.doc-title     → "PUBLICAÇÃO DA HOMOLOGAÇÃO DO SELETIVO"
    ///         └── href                  → link do edital
    /// </summary>
    private PublicacaoScrapingResult ExtrairSeletivaSeplagMT(HtmlDocument doc, string url)
    {
        // Encontra o primeiro card dentro da lista
        var cardNode = doc.DocumentNode.SelectSingleNode("//div[contains(@class, 'detalhes-doc-list')]//a[contains(@class, 'detalhes-doc-card')]");

        if (cardNode == null)
        {
            throw new InvalidOperationException(
                $"Nenhuma publicação encontrada em '{url}'. " +
                $"Verifique se a classe 'detalhes-doc-card' existe na página.");
        }

        // Extrai título (.doc-title ou tag p correspondente)
        var titleNode = cardNode.SelectSingleNode(".//p[contains(@class, 'detalhes-doc-title')]");
        if (titleNode == null)
        {
            // Tenta fallback com doc-title genérico
            titleNode = cardNode.SelectSingleNode(".//*[contains(@class, 'doc-title') or contains(@class, 'detalhes-doc-title')]");
        }
        var titulo = titleNode != null ? HtmlEntity.DeEntitize(titleNode.InnerText).Trim() : string.Empty;

        // Extrai data (.doc-date)
        var dateNode = cardNode.SelectSingleNode(".//span[contains(@class, 'detalhes-doc-date')]");
        if (dateNode == null)
        {
            // Tenta fallback com doc-date genérico
            dateNode = cardNode.SelectSingleNode(".//*[contains(@class, 'doc-date') or contains(@class, 'detalhes-doc-date')]");
        }
        var dataCompleta = dateNode != null ? HtmlEntity.DeEntitize(dateNode.InnerText).Trim() : string.Empty;

        // Extrai link (href do a)
        var link = cardNode.GetAttributeValue("href", string.Empty);
        if (!string.IsNullOrEmpty(link) && !link.StartsWith("http", StringComparison.OrdinalIgnoreCase))
        {
            // Se o link for relativo, resolvemos com base na URL do site
            var baseUri = new Uri(url);
            link = new Uri(baseUri, link).ToString();
        }

        // A data vem no formato "02/06/2026 · Download" — pega só a data
        var data = dataCompleta.Contains('·')
            ? dataCompleta.Split('·')[0].Trim()
            : dataCompleta.Contains('&') // Caso venha codificado como &middot; no InnerText
            ? dataCompleta.Split('&')[0].Trim()
            : dataCompleta.Trim();

        // Remove quaisquer outros caracteres residuais que representem o ponto médio de multiplicação (middot / ·)
        data = data.Replace("·", "").Trim();

        _logger.LogInformation("Publicação encontrada via HttpClient: [{Data}] {Titulo}", data, titulo);

        return new PublicacaoScrapingResult
        {
            Titulo = titulo,
            Data = data,
            Link = link
        };
    }
}
