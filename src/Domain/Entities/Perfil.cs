using Microsoft.AspNetCore.Identity;

namespace InfraStellar.Domain.Entities;

public class Perfil : IdentityRole<Guid>
{
    public Perfil() : base() { }

    public Perfil(string roleName) : base(roleName) { }
}
