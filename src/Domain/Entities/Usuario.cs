using Microsoft.AspNetCore.Identity;

namespace InfraStellar.Domain.Entities;

public class Usuario : IdentityUser<Guid>
{
    public string Nome { get; set; } = string.Empty;
    public Guid? AvatarId { get; set; }
    public Avatar? Avatar { get; set; }
}
