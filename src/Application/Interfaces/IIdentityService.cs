using InfraStellar.Application.DTOs;

namespace InfraStellar.Application.Interfaces;

public interface IIdentityService
{
    Task<bool> RegistrarUsuarioAsync(CadastroInputDto dto);
    Task<bool> LogarUsuarioAsync(LoginInputDto dto);
    Task DeslogarUsuarioAsync();
}
