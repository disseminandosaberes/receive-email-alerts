using InfraStellar.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;

namespace InfraStellar.WebRazor.Pages.Alertas;

[Authorize]
public class IndexModel : PageModel
{
    private readonly IAlertaService _alertaService;

    public IndexModel(IAlertaService alertaService)
    {
        _alertaService = alertaService;
    }

    public List<Application.DTOs.AlertaDto> Alertas { get; set; } = [];

    [TempData] public string? MensagemSucesso { get; set; }
    [TempData] public string? MensagemErro { get; set; }

    public async Task OnGetAsync()
    {
        Alertas = await _alertaService.ListarAlertasDoUsuarioAsync(ObterUsuarioId());
    }

    public async Task<IActionResult> OnPostExcluirAsync(Guid alertaId)
    {
        try
        {
            await _alertaService.ExcluirAlertaAsync(alertaId, ObterUsuarioId());
            MensagemSucesso = "Alerta excluído com sucesso.";
        }
        catch (Exception ex) { MensagemErro = ex.Message; }
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostExecutarAsync(Guid alertaId)
    {
        try
        {
            var resultado = await _alertaService.ExecutarAlertaAsync(alertaId, ObterUsuarioId());

            if (!resultado.Sucesso)
            {
                MensagemErro = $"Falha técnica: {resultado.MensagemErro}";
            }
            else if (resultado.NovaPublicacaoDetectada)
            {
                MensagemSucesso = $"🆕 Nova publicação detectada: [{resultado.DataPublicacao}] {resultado.TituloPublicacao}";
            }
            else
            {
                MensagemSucesso = $"✅ Sem novidades. Última publicação: [{resultado.DataPublicacao}] {resultado.TituloPublicacao}";
            }
        }
        catch (Exception ex) { MensagemErro = $"Erro: {ex.Message}"; }
        return RedirectToPage();
    }

    private Guid ObterUsuarioId() =>
        Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? throw new InvalidOperationException("Usuário não autenticado."));
}
