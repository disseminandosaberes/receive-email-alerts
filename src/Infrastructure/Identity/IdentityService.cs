using InfraStellar.Application.DTOs;
using InfraStellar.Application.Interfaces;
using InfraStellar.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace InfraStellar.Infrastructure.Identity;

public class IdentityService : IIdentityService
{
    private readonly UserManager<Usuario> _userManager;
    private readonly SignInManager<Usuario> _signInManager;

    public IdentityService(UserManager<Usuario> userManager, SignInManager<Usuario> signInManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
    }

    public async Task<bool> RegistrarUsuarioAsync(CadastroInputDto dto)
    {
        var usuario = new Usuario
        {
            UserName = dto.Email,
            Email = dto.Email,
            Nome = dto.Nome
        };

        var resultado = await _userManager.CreateAsync(usuario, dto.Senha);
        return resultado.Succeeded;
    }

    public async Task<bool> LogarUsuarioAsync(LoginInputDto dto)
    {
        var resultado = await _signInManager.PasswordSignInAsync(
            userName: dto.Email,
            password: dto.Senha,
            isPersistent: dto.LembrarMe,
            lockoutOnFailure: false);

        return resultado.Succeeded;
    }

    public async Task DeslogarUsuarioAsync()
    {
        await _signInManager.SignOutAsync();
    }
}
