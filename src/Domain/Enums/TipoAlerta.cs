namespace InfraStellar.Domain.Enums;

/// <summary>
/// Representa o tipo de alerta baseado no domínio do site monitorado.
/// Cada valor mapeia para uma estratégia de scraping específica que conhece
/// a estrutura HTML do respectivo site.
/// </summary>
public enum TipoAlerta
{
    /// <summary>
    /// Portal de Seletivos da SEPLAG-MT.
    /// Domínio: seletivo.seplag.mt.gov.br
    /// Monitora a lista de documentos/editais publicados na página de detalhe de um seletivo.
    /// </summary>
    SeletivaSeplagMT = 0,
}
