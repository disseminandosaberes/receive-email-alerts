using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InfraStellar.Domain.Entities;
using InfraStellar.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace InfraStellar.WebRazor.Pages.Admin;

[Authorize(Roles = "Admin")]
public class DashboardModel : PageModel
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<Usuario> _userManager;
    private readonly RoleManager<Perfil> _roleManager;

    public DashboardModel(
        ApplicationDbContext context,
        UserManager<Usuario> userManager,
        RoleManager<Perfil> roleManager)
    {
        _context = context;
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public int TotalUsers { get; set; }
    public int TotalRoles { get; set; }
    public List<SolicitacaoRole> PendingRequests { get; set; } = new();

    public async Task OnGetAsync()
    {
        TotalUsers = await _context.Users.CountAsync();
        TotalRoles = await _context.Roles.CountAsync();
        PendingRequests = await _context.SolicitacoesRole
            .Include(r => r.Usuario)
            .Where(r => r.Status == "Pendente")
            .OrderBy(r => r.DataSolicitacao)
            .ToListAsync();
    }

    public async Task<IActionResult> OnPostApproveRequestAsync(Guid id)
    {
        var request = await _context.SolicitacoesRole
            .Include(r => r.Usuario)
            .FirstOrDefaultAsync(r => r.Id == id);

        if (request == null || request.Status != "Pendente" || request.Usuario == null)
        {
            return RedirectToPage();
        }

        // Garante que a role exista
        if (!await _roleManager.RoleExistsAsync(request.RoleNome))
        {
            await _roleManager.CreateAsync(new Perfil(request.RoleNome));
        }

        // Associa o usuário à role
        var result = await _userManager.AddToRoleAsync(request.Usuario, request.RoleNome);
        if (result.Succeeded)
        {
            request.Status = "Aprovada";
            request.DataProcessamento = DateTime.UtcNow;
            request.ProcessadoPor = User.Identity?.Name;
            await _context.SaveChangesAsync();
        }

        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostRejectRequestAsync(Guid id)
    {
        var request = await _context.SolicitacoesRole
            .FirstOrDefaultAsync(r => r.Id == id);

        if (request == null || request.Status != "Pendente")
        {
            return RedirectToPage();
        }

        request.Status = "Rejeitada";
        request.DataProcessamento = DateTime.UtcNow;
        request.ProcessadoPor = User.Identity?.Name;
        await _context.SaveChangesAsync();

        return RedirectToPage();
    }
}
