using InfraStellar.Application.DTOs;
using InfraStellar.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace InfraStellar.WebRazor.Pages.Account;

public class LoginModel : PageModel
{
    private readonly IIdentityService _identityService;

    public LoginModel(IIdentityService identityService)
    {
        _identityService = identityService;
    }

    [BindProperty]
    public LoginInputDto Input { get; set; } = new();

    public bool CadastroRealizadoComSucesso { get; private set; }

    public IActionResult OnGet(bool? registered)
    {
        if (User.Identity?.IsAuthenticated == true)
            return RedirectToPage("/Index");

        CadastroRealizadoComSucesso = registered == true;
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
            return Page();

        var sucesso = await _identityService.LogarUsuarioAsync(Input);

        if (!sucesso)
        {
            ModelState.AddModelError(string.Empty, "E-mail ou senha inválidos. Verifique suas credenciais e tente novamente.");
            return Page();
        }

        return RedirectToPage("/Index");
    }
}
