using InfraStellar.Application.Interfaces;
using InfraStellar.Domain.Entities;
using InfraStellar.Infrastructure.BackgroundServices;
using InfraStellar.Infrastructure.Data;
using InfraStellar.Infrastructure.Identity;
using InfraStellar.Infrastructure.Services;
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

        // Registra o HttpClient para realizar requisições leves de scraping
        services.AddHttpClient();

        // Registra o serviço de scraping (HttpClient + HtmlAgilityPack)
        services.AddScoped<IScrapingService, ScrapingService>();

        // Registra o serviço de notificação (mock — retorna true por enquanto)
        services.AddScoped<INotificacaoService, NotificacaoService>();

        // Registra o AlertaService (como interface e como concreto — background service usa o concreto)
        services.AddScoped<AlertaService>();
        services.AddScoped<IAlertaService>(sp => sp.GetRequiredService<AlertaService>());

        // Background service que executa alertas automaticamente em segundo plano
        services.AddHostedService<AlertaBackgroundService>();

        return services;
    }
}
