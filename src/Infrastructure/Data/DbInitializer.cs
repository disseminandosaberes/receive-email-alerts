using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using InfraStellar.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace InfraStellar.Infrastructure.Data;

public static class DbInitializer
{
    public static async Task SeedAsync(
        ApplicationDbContext context,
        UserManager<Usuario> userManager,
        RoleManager<Perfil> roleManager,
        string webRootPath)
    {
        // Garante que as migrações sejam aplicadas
        await context.Database.MigrateAsync();

        // 1. Semear Roles (Perfis)
        string[] roles = { "Admin", "User" };
        foreach (var roleName in roles)
        {
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                await roleManager.CreateAsync(new Perfil(roleName));
            }
        }

        // 2. Semear Usuário Admin Padrão
        var adminEmail = "admin@admin.com";
        var adminUser = await userManager.FindByEmailAsync(adminEmail);
        if (adminUser == null)
        {
            adminUser = new Usuario
            {
                UserName = adminEmail,
                Email = adminEmail,
                Nome = "Administrador",
                EmailConfirmed = true
            };

            var result = await userManager.CreateAsync(adminUser, "Admin@123");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(adminUser, "Admin");
            }
        }

        // 3. Semear Avatares
        if (!await context.Avatares.AnyAsync())
        {
            var avatarsFolder = Path.Combine(webRootPath, "assets", "avatars");
            if (Directory.Exists(avatarsFolder))
            {
                for (int i = 1; i <= 15; i++)
                {
                    var filePath = Path.Combine(avatarsFolder, $"avatar{i}.png");
                    if (File.Exists(filePath))
                    {
                        var fileBytes = await File.ReadAllBytesAsync(filePath);
                        var avatar = new Avatar
                        {
                            Id = Guid.NewGuid(),
                            Nome = $"Avatar {i}",
                            Dados = fileBytes,
                            ContentType = "image/png"
                        };
                        context.Avatares.Add(avatar);
                    }
                }
                await context.SaveChangesAsync();
            }
        }
    }
}
