using InfraStellar.Application.DTOs;
using InfraStellar.Application.Interfaces;
using InfraStellar.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;

namespace InfraStellar.WebRazor.Pages.Alertas;

[Authorize]
public class CriarModel : PageModel
{
    private readonly IAlertaService _alertaService;

    public CriarModel(IAlertaService alertaService)
    {
        _alertaService = alertaService;
    }

    [BindProperty]
    public CriarAlertaDto Input { get; set; } = new();

    /// <summary>Exemplos de URL por tipo de alerta.</summary>
    public Dictionary<TipoAlerta, string> ExemplosUrl { get; } = new()
    {
        [TipoAlerta.SeletivaSeplagMT] = "https://seletivo.seplag.mt.gov.br/detalhes/51?slug=prt-no-0012026seplag-programa-de-residencia-tecnica"
    };

    public void OnGet() { }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
            return Page();

        try
        {
            var usuarioId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            await _alertaService.CriarAlertaAsync(Input, usuarioId);
            TempData["MensagemSucesso"] = $"Alerta \"{Input.Nome}\" criado com sucesso!";
            return RedirectToPage("/Alertas/Index");
        }
        catch (Exception ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            return Page();
        }
    }
}
