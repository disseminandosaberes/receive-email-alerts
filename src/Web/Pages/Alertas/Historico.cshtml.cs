using InfraStellar.Application.Interfaces;
using InfraStellar.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;

namespace InfraStellar.WebRazor.Pages.Alertas;

[Authorize]
public class HistoricoModel : PageModel
{
    private readonly IAlertaService _alertaService;

    public HistoricoModel(IAlertaService alertaService)
    {
        _alertaService = alertaService;
    }

    [BindProperty(SupportsGet = true)]
    public Guid AlertaId { get; set; }

    [BindProperty(SupportsGet = true)]
    public int Pagina { get; set; } = 1;

    public List<ResultadoAlerta> Resultados { get; set; } = [];
    public string AlertaNome { get; set; } = string.Empty;

    public async Task<IActionResult> OnGetAsync()
    {
        if (AlertaId == Guid.Empty)
            return RedirectToPage("/Alertas/Index");

        var usuarioId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        try
        {
            Resultados = await _alertaService.HistoricoAsync(AlertaId, usuarioId, Pagina);

            // Pega o nome do alerta para exibir no título
            var alertas = await _alertaService.ListarAlertasDoUsuarioAsync(usuarioId);
            AlertaNome = alertas.FirstOrDefault(a => a.Id == AlertaId)?.Nome ?? "Alerta";
        }
        catch
        {
            return RedirectToPage("/Alertas/Index");
        }

        return Page();
    }
}
