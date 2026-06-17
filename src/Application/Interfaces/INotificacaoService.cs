namespace InfraStellar.Application.Interfaces;

/// <summary>
/// Interface para o serviço de notificação.
/// Responsável por enviar mensagens ao usuário quando uma nova publicação é detectada.
/// A implementação atual é um mock que retorna true — a integração real (e-mail, push, etc.)
/// será implementada em uma próxima iteração.
/// </summary>
public interface INotificacaoService
{
    /// <summary>
    /// Envia uma notificação para o destinatário informando sobre uma nova publicação.
    /// </summary>
    /// <param name="destinatario">E-mail ou identificador do destinatário.</param>
    /// <param name="tituloAlerta">Nome do alerta que disparou a notificação.</param>
    /// <param name="tituloPublicacao">Título da nova publicação detectada.</param>
    /// <param name="dataPublicacao">Data da nova publicação.</param>
    /// <param name="linkPublicacao">Link opcional para o documento.</param>
    /// <returns>True se a notificação foi enviada com sucesso.</returns>
    Task<bool> EnviarNotificacaoAsync(
        string destinatario,
        string tituloAlerta,
        string tituloPublicacao,
        string dataPublicacao,
        string? linkPublicacao = null);
}
