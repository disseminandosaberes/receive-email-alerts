using InfraStellar.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace InfraStellar.WebRazor.Pages.Account;

public class LogoutModel : PageModel
{
    private readonly IIdentityService _identityService;

    public LogoutModel(IIdentityService identityService)
    {
        _identityService = identityService;
    }

    public IActionResult OnGet()
    {
        // GET em /Account/Logout redireciona para home (não faz logout via GET por segurança)
        return RedirectToPage("/Index");
    }

    public async Task<IActionResult> OnPostAsync()
    {
        await _identityService.DeslogarUsuarioAsync();
        return RedirectToPage("/Account/Login");
    }
}
