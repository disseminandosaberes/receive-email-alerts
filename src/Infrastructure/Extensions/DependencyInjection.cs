using InfraStellar.Application.Interfaces;
using InfraStellar.Domain.Entities;
using InfraStellar.Infrastructure.Data;
using InfraStellar.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace InfraStellar.Infrastructure.Extensions;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services,
        IConfiguration config)
    {
        // Registra o DbContext com SQL Server
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(config.GetConnectionString("DefaultConnection")));

        // Configura o ASP.NET Core Identity com Guid como chave
        services.AddIdentity<Usuario, Perfil>(options =>
        {
            options.Password.RequireDigit = true;
            options.Password.RequiredLength = 6;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequireUppercase = false;
            options.Password.RequireLowercase = false;

            options.SignIn.RequireConfirmedAccount = false;
        })
        .AddEntityFrameworkStores<ApplicationDbContext>()
        .AddDefaultTokenProviders()
        .AddClaimsPrincipalFactory<CustomClaimsPrincipalFactory>();

        // Registra a implementação concreta do serviço de identidade
        services.AddScoped<IIdentityService, IdentityService>();

        return services;
    }
}
