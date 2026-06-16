using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace InfraStellar.WebRazor.Pages.Admin;

[Authorize(Roles = "Admin")]
public class DashboardModel : PageModel
{
    public void OnGet() { }
}
