using InfraStellar.Application.DTOs;
using InfraStellar.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace InfraStellar.WebRazor.Pages.Account;

public class RegisterModel : PageModel
{
    private readonly IIdentityService _identityService;

    public RegisterModel(IIdentityService identityService)
    {
        _identityService = identityService;
    }

    [BindProperty]
    public CadastroInputDto Input { get; set; } = new();

    public IActionResult OnGet()
    {
        // Se o usuário já está autenticado, redireciona para home
        if (User.Identity?.IsAuthenticated == true)
            return RedirectToPage("/Index");

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
            return Page();

        var sucesso = await _identityService.RegistrarUsuarioAsync(Input);

        if (!sucesso)
        {
            ModelState.AddModelError(string.Empty, "Não foi possível criar a conta. O e-mail pode já estar em uso ou a senha não atende aos requisitos.");
            return Page();
        }

        return RedirectToPage("/Account/Login", new { registered = true });
    }
}
