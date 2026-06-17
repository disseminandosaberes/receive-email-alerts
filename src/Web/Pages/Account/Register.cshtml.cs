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

        if (await _identityService.EmailJaCadastradoAsync(Input.Email))
        {
            ModelState.AddModelError("Input.Email", "Este e-mail já está cadastrado.");
            return Page();
        }

        var sucesso = await _identityService.RegistrarUsuarioAsync(Input);

        if (!sucesso)
        {
            ModelState.AddModelError(string.Empty, "Não foi possível criar a conta. A senha pode não atender aos requisitos exigidos.");
            return Page();
        }

        return RedirectToPage("/Account/Login", new { registered = true });
    }
}
