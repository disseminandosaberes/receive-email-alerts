using System.ComponentModel.DataAnnotations;
using InfraStellar.Domain.Enums;

namespace InfraStellar.Application.DTOs;

public class CriarAlertaDto
{
    [Required(ErrorMessage = "Informe um nome para o alerta.")]
    [StringLength(100, ErrorMessage = "Máximo de 100 caracteres.")]
    public string Nome { get; set; } = string.Empty;

    [Required(ErrorMessage = "Selecione o tipo do alerta.")]
    public TipoAlerta TipoAlerta { get; set; }

    [Required(ErrorMessage = "Informe a URL da página a monitorar.")]
    [Url(ErrorMessage = "Informe uma URL válida (ex: https://seletivo.seplag.mt.gov.br/detalhes/51?slug=...).")]
    [StringLength(2000)]
    public string Url { get; set; } = string.Empty;

    [Required(ErrorMessage = "Selecione o intervalo de monitoramento.")]
    [Range(1, 10080, ErrorMessage = "O intervalo deve ser de 1 minuto até 7 dias.")]
    public int IntervaloMinutos { get; set; } = 60;
}
