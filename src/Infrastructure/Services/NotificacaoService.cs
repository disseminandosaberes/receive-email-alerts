using InfraStellar.Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace InfraStellar.Infrastructure.Services;

/// <summary>
/// Implementação mock de <see cref="INotificacaoService"/>.
/// Registra a notificação no log e retorna true.
/// A integração real (e-mail, WhatsApp, push notification, etc.)
/// será implementada em uma próxima iteração.
/// </summary>
public class NotificacaoService : INotificacaoService
{
    private readonly ILogger<NotificacaoService> _logger;

    public NotificacaoService(ILogger<NotificacaoService> logger)
    {
        _logger = logger;
    }

    public Task<bool> EnviarNotificacaoAsync(
        string destinatario,
        string tituloAlerta,
        string tituloPublicacao,
        string dataPublicacao,
        string? linkPublicacao = null)
    {
        _logger.LogInformation(
            "[NOTIFICAÇÃO MOCK] Para: {Dest} | Alerta: {Alerta} | Nova publicação: [{Data}] {Titulo} | Link: {Link}",
            destinatario,
            tituloAlerta,
            dataPublicacao,
            tituloPublicacao,
            linkPublicacao ?? "—");

        // TODO: implementar envio real (e-mail SMTP, SendGrid, SignalR push, etc.)
        return Task.FromResult(true);
    }
}
