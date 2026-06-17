using InfraStellar.Infrastructure.Data;
using InfraStellar.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace InfraStellar.Infrastructure.BackgroundServices;

/// <summary>
/// Background service que executa periodicamente todos os alertas ativos.
/// Intervalo configurável em appsettings.json via "Alertas:IntervaloMinutos".
/// </summary>
public class AlertaBackgroundService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<AlertaBackgroundService> _logger;
    private readonly TimeSpan _intervalo;

    public AlertaBackgroundService(
        IServiceScopeFactory scopeFactory,
        ILogger<AlertaBackgroundService> logger,
        IConfiguration configuration)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
        // Frequência de verificação da fila em background (padrão 1 minuto)
        var minutos = configuration.GetValue<int>("Alertas:FrequenciaVerificacaoMinutos", 1);
        _intervalo = TimeSpan.FromMinutes(minutos);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("AlertaBackgroundService iniciado. Frequência de verificação: {Intervalo}", _intervalo);
        await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);

        while (!stoppingToken.IsCancellationRequested)
        {
            await ProcessarAlertasAtivosAsync(stoppingToken);
            await Task.Delay(_intervalo, stoppingToken);
        }
    }

    private async Task ProcessarAlertasAtivosAsync(CancellationToken ct)
    {
        _logger.LogInformation("Processando alertas ativos em background...");

        using var scope = _scopeFactory.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var alertaService = scope.ServiceProvider.GetRequiredService<AlertaService>();

        var alertasAtivos = await db.Alertas
            .Include(a => a.Usuario)
            .Where(a => a.Ativo)
            .ToListAsync(ct);

        var agora = DateTimeOffset.UtcNow;
        var alertasParaExecutar = alertasAtivos
            .Where(a => a.UltimaExecucaoEm == null || 
                        a.UltimaExecucaoEm.Value.AddMinutes(a.IntervaloMinutos) <= agora)
            .ToList();

        _logger.LogInformation("{Count} alertas ativos encontrados. {ExecuteCount} agendados para executar agora.", 
            alertasAtivos.Count, alertasParaExecutar.Count);

        foreach (var alerta in alertasParaExecutar)
        {
            if (ct.IsCancellationRequested) break;

            try
            {
                var resultado = await alertaService.ExecutarInternamenteAsync(alerta);
                _logger.LogInformation(
                    "Alerta '{Nome}' processado. Nova publicação: {Nova}. Sucesso: {Sucesso}",
                    alerta.Nome, resultado.NovaPublicacaoDetectada, resultado.Sucesso);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao processar alerta '{Nome}' ({Id})", alerta.Nome, alerta.Id);
            }
        }
    }
}
