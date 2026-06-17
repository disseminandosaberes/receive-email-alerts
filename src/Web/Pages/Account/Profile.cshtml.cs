using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using InfraStellar.Domain.Entities;
using InfraStellar.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace InfraStellar.WebRazor.Pages.Account;

[Authorize]
public class ProfileModel : PageModel
{
    private readonly UserManager<Usuario> _userManager;
    private readonly SignInManager<Usuario> _signInManager;
    private readonly ApplicationDbContext _context;

    public ProfileModel(
        UserManager<Usuario> userManager,
        SignInManager<Usuario> signInManager,
        ApplicationDbContext context)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _context = context;
    }

    [BindProperty]
    public InputModel Input { get; set; } = new();

    public class InputModel
    {
        [Required(ErrorMessage = "O nome é obrigatório.")]
        [MaxLength(150, ErrorMessage = "O nome deve ter no máximo 150 caracteres.")]
        public string Nome { get; set; } = string.Empty;

        public Guid? SelectedAvatarId { get; set; }
    }

    public string Email { get; set; } = string.Empty;
    public IList<string> CurrentRoles { get; set; } = new List<string>();
    public List<Avatar> AvailableAvatars { get; set; } = new();
    public List<SolicitacaoRole> UserRoleRequests { get; set; } = new();

    [TempData]
    public string StatusMessage { get; set; } = string.Empty;

    public async Task<IActionResult> OnGetAsync()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return NotFound($"Não foi possível carregar o usuário com ID '{_userManager.GetUserId(User)}'.");
        }

        Email = user.Email ?? string.Empty;
        Input.Nome = user.Nome;
        Input.SelectedAvatarId = user.AvatarId;

        CurrentRoles = await _userManager.GetRolesAsync(user);
        AvailableAvatars = await _context.Avatares.ToListAsync();
        UserRoleRequests = await _context.SolicitacoesRole
            .Where(r => r.UsuarioId == user.Id)
            .OrderByDescending(r => r.DataSolicitacao)
            .ToListAsync();

        return Page();
    }

    public async Task<IActionResult> OnPostUpdateProfileAsync()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return NotFound($"Não foi possível carregar o usuário.");
        }

        if (!ModelState.IsValid)
        {
            // Recarrega os dados necessários para a página
            Email = user.Email ?? string.Empty;
            CurrentRoles = await _userManager.GetRolesAsync(user);
            AvailableAvatars = await _context.Avatares.ToListAsync();
            UserRoleRequests = await _context.SolicitacoesRole
                .Where(r => r.UsuarioId == user.Id)
                .OrderByDescending(r => r.DataSolicitacao)
                .ToListAsync();
            return Page();
        }

        user.Nome = Input.Nome;
        user.AvatarId = Input.SelectedAvatarId;

        var result = await _userManager.UpdateAsync(user);
        if (result.Succeeded)
        {
            // Atualiza os cookies/claims da sessão ativa para refletir o novo nome e avatar sem deslogar
            await _signInManager.RefreshSignInAsync(user);
            StatusMessage = "Seu perfil foi atualizado com sucesso!";
        }
        else
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
            // Recarrega os dados
            Email = user.Email ?? string.Empty;
            CurrentRoles = await _userManager.GetRolesAsync(user);
            AvailableAvatars = await _context.Avatares.ToListAsync();
            UserRoleRequests = await _context.SolicitacoesRole
                .Where(r => r.UsuarioId == user.Id)
                .OrderByDescending(r => r.DataSolicitacao)
                .ToListAsync();
            return Page();
        }

        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostRequestRoleAsync(string roleName)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return NotFound("Usuário não encontrado.");
        }

        if (string.IsNullOrWhiteSpace(roleName))
        {
            StatusMessage = "Erro: Nome da role inválido.";
            return RedirectToPage();
        }

        // Verifica se o usuário já possui a role
        var jaPossui = await _userManager.IsInRoleAsync(user, roleName);
        if (jaPossui)
        {
            StatusMessage = $"Você já possui a permissão de {roleName}.";
            return RedirectToPage();
        }

        // Verifica se já existe uma solicitação pendente para esta role
        var jaSolicitou = await _context.SolicitacoesRole
            .AnyAsync(r => r.UsuarioId == user.Id && r.RoleNome == roleName && r.Status == "Pendente");

        if (jaSolicitou)
        {
            StatusMessage = $"Você já possui uma solicitação pendente para a permissão de {roleName}.";
            return RedirectToPage();
        }

        var solicitacao = new SolicitacaoRole
        {
            Id = Guid.NewGuid(),
            UsuarioId = user.Id,
            RoleNome = roleName,
            Status = "Pendente",
            DataSolicitacao = DateTime.UtcNow
        };

        _context.SolicitacoesRole.Add(solicitacao);
        await _context.SaveChangesAsync();

        StatusMessage = $"Solicitação para o perfil '{roleName}' enviada com sucesso! Aguarde aprovação de um administrador.";
        return RedirectToPage();
    }

    // Handler de Imagem para carregar o avatar dinamicamente a partir dos bytes salvos no BD
    public async Task<IActionResult> OnGetAvatarAsync(Guid id)
    {
        var avatar = await _context.Avatares.FindAsync(id);
        if (avatar == null || avatar.Dados == null || avatar.Dados.Length == 0)
        {
            // Retorna um arquivo vazio ou placeholder se não achar
            return NotFound();
        }
        return File(avatar.Dados, avatar.ContentType);
    }
}
