using InfraStellar.Application.DTOs;
using InfraStellar.Application.Interfaces;
using InfraStellar.Domain.Entities;
using InfraStellar.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace InfraStellar.Infrastructure.Services;

public class AlertaService : IAlertaService
{
    private readonly ApplicationDbContext _db;
    private readonly IScrapingService _scraping;
    private readonly INotificacaoService _notificacao;
    private readonly ILogger<AlertaService> _logger;

    public AlertaService(
        ApplicationDbContext db,
        IScrapingService scraping,
        INotificacaoService notificacao,
        ILogger<AlertaService> logger)
    {
        _db = db;
        _scraping = scraping;
        _notificacao = notificacao;
        _logger = logger;
    }

    public async Task<List<AlertaDto>> ListarAlertasDoUsuarioAsync(Guid usuarioId)
    {
        var alertas = await _db.Alertas
            .Where(a => a.UsuarioId == usuarioId)
            .OrderByDescending(a => a.CriadoEm)
            .Select(a => new
            {
                a.Id, a.Nome, a.TipoAlerta, a.Url, a.Ativo, a.CriadoEm,
                a.UltimoTituloConhecido, a.UltimaDataConhecida,
                a.IntervaloMinutos, a.UltimaExecucaoEm,
                Ultimo = a.Resultados
                    .OrderByDescending(r => r.ExecutadoEm)
                    .FirstOrDefault()
            })
            .ToListAsync();

        return alertas.Select(a => new AlertaDto
        {
            Id = a.Id,
            Nome = a.Nome,
            TipoAlerta = a.TipoAlerta,
            Url = a.Url,
            Ativo = a.Ativo,
            CriadoEm = a.CriadoEm,
            UltimoTitulo = a.UltimoTituloConhecido,
            UltimaData = a.UltimaDataConhecida,
            IntervaloMinutos = a.IntervaloMinutos,
            UltimaExecucaoEm = a.UltimaExecucaoEm,
            UltimaExecucao = a.Ultimo?.ExecutadoEm,
            UltimoSucesso = a.Ultimo?.Sucesso,
            UltimaNovaPublicacao = a.Ultimo?.NovaPublicacaoDetectada,
            UltimoLink = a.Ultimo?.LinkPublicacao
        }).ToList();
    }

    public async Task CriarAlertaAsync(CriarAlertaDto dto, Guid usuarioId)
    {
        var alerta = new Alerta
        {
            UsuarioId = usuarioId,
            Nome = dto.Nome,
            TipoAlerta = dto.TipoAlerta,
            Url = dto.Url,
            IntervaloMinutos = dto.IntervaloMinutos,
            Ativo = true,
            CriadoEm = DateTimeOffset.UtcNow
        };

        _db.Alertas.Add(alerta);
        await _db.SaveChangesAsync();

        _logger.LogInformation("Alerta '{Nome}' criado para usuário {UsuarioId}", dto.Nome, usuarioId);
    }

    public async Task ExcluirAlertaAsync(Guid alertaId, Guid usuarioId)
    {
        var alerta = await _db.Alertas
            .FirstOrDefaultAsync(a => a.Id == alertaId && a.UsuarioId == usuarioId)
            ?? throw new InvalidOperationException("Alerta não encontrado ou sem permissão.");

        _db.Alertas.Remove(alerta);
        await _db.SaveChangesAsync();
    }

    public async Task<ResultadoAlerta> ExecutarAlertaAsync(Guid alertaId, Guid usuarioId)
    {
        var alerta = await _db.Alertas
            .Include(a => a.Usuario)
            .FirstOrDefaultAsync(a => a.Id == alertaId && a.UsuarioId == usuarioId)
            ?? throw new InvalidOperationException("Alerta não encontrado ou sem permissão.");

        return await ExecutarInternamenteAsync(alerta);
    }

    /// <summary>
    /// Lógica central de execução — usada pelo disparo manual e pelo background service.
    /// </summary>
    public async Task<ResultadoAlerta> ExecutarInternamenteAsync(Alerta alerta)
    {
        var resultado = new ResultadoAlerta
        {
            AlertaId = alerta.Id,
            ExecutadoEm = DateTimeOffset.UtcNow
        };

        try
        {
            var publicacao = await _scraping.ExtrairPublicacaoMaisRecenteAsync(alerta.Url, alerta.TipoAlerta);

            resultado.TituloPublicacao = publicacao.Titulo;
            resultado.DataPublicacao = publicacao.Data;
            resultado.LinkPublicacao = publicacao.Link;
            resultado.Sucesso = true;

            // Detecta se é uma publicação nova em relação ao último estado conhecido
            var isNova = alerta.UltimoTituloConhecido is not null
                && !string.Equals(alerta.UltimoTituloConhecido, publicacao.Titulo, StringComparison.OrdinalIgnoreCase);

            resultado.NovaPublicacaoDetectada = isNova;

            if (isNova)
            {
                _logger.LogInformation(
                    "🆕 Nova publicação detectada no alerta '{Nome}': [{Data}] {Titulo}",
                    alerta.Nome, publicacao.Data, publicacao.Titulo);

                // Envia notificação (mock por enquanto — retorna true)
                var destinatario = alerta.Usuario?.Email ?? alerta.UsuarioId.ToString();
                await _notificacao.EnviarNotificacaoAsync(
                    destinatario,
                    alerta.Nome,
                    publicacao.Titulo,
                    publicacao.Data,
                    publicacao.Link);
            }

            // Atualiza o estado do alerta com a publicação mais recente encontrada
            alerta.UltimoTituloConhecido = publicacao.Titulo;
            alerta.UltimaDataConhecida = publicacao.Data;
        }
        catch (Exception ex)
        {
            resultado.Sucesso = false;
            resultado.MensagemErro = ex.Message;
            _logger.LogError(ex, "Falha ao executar alerta '{Nome}' ({Id})", alerta.Nome, alerta.Id);
        }

        alerta.UltimaExecucaoEm = resultado.ExecutadoEm;

        _db.ResultadosAlerta.Add(resultado);
        await _db.SaveChangesAsync();

        return resultado;
    }

    public async Task<List<ResultadoAlerta>> HistoricoAsync(
        Guid alertaId, Guid usuarioId, int pagina = 1, int tamanhoPagina = 20)
    {
        var existe = await _db.Alertas
            .AnyAsync(a => a.Id == alertaId && a.UsuarioId == usuarioId);

        if (!existe)
            throw new InvalidOperationException("Alerta não encontrado ou sem permissão.");

        return await _db.ResultadosAlerta
            .Where(r => r.AlertaId == alertaId)
            .OrderByDescending(r => r.ExecutadoEm)
            .Skip((pagina - 1) * tamanhoPagina)
            .Take(tamanhoPagina)
            .ToListAsync();
    }
}
