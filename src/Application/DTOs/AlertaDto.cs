using InfraStellar.Domain.Enums;

namespace InfraStellar.Application.DTOs;

public class AlertaDto
{
    public Guid Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public TipoAlerta TipoAlerta { get; set; }

    public string TipoAlertaDescricao => TipoAlerta switch
    {
        TipoAlerta.SeletivaSeplagMT => "Seletivo SEPLAG-MT",
        _ => TipoAlerta.ToString()
    };

    public string TipoAlertaDominio => TipoAlerta switch
    {
        TipoAlerta.SeletivaSeplagMT => "seletivo.seplag.mt.gov.br",
        _ => "—"
    };

    public string Url { get; set; } = string.Empty;
    public bool Ativo { get; set; }
    public DateTimeOffset CriadoEm { get; set; }

    // --- Estado atual do monitoramento ---
    public string? UltimoTitulo { get; set; }
    public string? UltimaData { get; set; }

    public int IntervaloMinutos { get; set; }
    public DateTimeOffset? UltimaExecucaoEm { get; set; }

    public string IntervaloDescricao => IntervaloMinutos switch
    {
        5 => "a cada 5 minutos",
        15 => "a cada 15 minutos",
        30 => "a cada 30 minutos",
        60 => "a cada 1 hora",
        180 => "a cada 3 horas",
        360 => "a cada 6 horas",
        720 => "a cada 12 horas",
        1440 => "diariamente",
        _ => $"a cada {IntervaloMinutos} minutos"
    };

    // --- Último resultado de execução ---
    public DateTimeOffset? UltimaExecucao { get; set; }
    public bool? UltimoSucesso { get; set; }
    public bool? UltimaNovaPublicacao { get; set; }
    public string? UltimoLink { get; set; }
}
