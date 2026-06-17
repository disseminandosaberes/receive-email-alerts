using InfraStellar.Application.DTOs;
using InfraStellar.Domain.Entities;

namespace InfraStellar.Application.Interfaces;

public interface IAlertaService
{
    /// <summary>Retorna todos os alertas do usuário com o estado atual e último resultado.</summary>
    Task<List<AlertaDto>> ListarAlertasDoUsuarioAsync(Guid usuarioId);

    /// <summary>Cria um novo alerta para o usuário.</summary>
    Task CriarAlertaAsync(CriarAlertaDto dto, Guid usuarioId);

    /// <summary>Exclui o alerta se pertencer ao usuário.</summary>
    Task ExcluirAlertaAsync(Guid alertaId, Guid usuarioId);

    /// <summary>
    /// Executa o monitoramento do alerta imediatamente (disparo manual):
    /// faz scraping da publicação mais recente, compara com a última conhecida,
    /// se nova → dispara notificação e atualiza estado no banco.
    /// </summary>
    Task<ResultadoAlerta> ExecutarAlertaAsync(Guid alertaId, Guid usuarioId);

    /// <summary>Retorna o histórico paginado de resultados de um alerta.</summary>
    Task<List<ResultadoAlerta>> HistoricoAsync(Guid alertaId, Guid usuarioId, int pagina = 1, int tamanhoPagina = 20);
}
