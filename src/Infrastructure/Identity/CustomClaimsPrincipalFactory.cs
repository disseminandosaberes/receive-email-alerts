using System.Security.Claims;
using InfraStellar.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace InfraStellar.Infrastructure.Identity;

public class CustomClaimsPrincipalFactory : UserClaimsPrincipalFactory<Usuario, Perfil>
{
    public CustomClaimsPrincipalFactory(
        UserManager<Usuario> userManager,
        RoleManager<Perfil> roleManager,
        IOptions<IdentityOptions> optionsAccessor)
        : base(userManager, roleManager, optionsAccessor)
    {
    }

    protected override async Task<ClaimsIdentity> GenerateClaimsAsync(Usuario user)
    {
        var identity = await base.GenerateClaimsAsync(user);
        identity.AddClaim(new Claim("Nome", user.Nome ?? string.Empty));
        identity.AddClaim(new Claim("AvatarId", user.AvatarId?.ToString() ?? string.Empty));
        return identity;
    }
}
