using InfraStellar.Infrastructure.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

// Registra toda a infraestrutura (DbContext, Identity, IIdentityService)
builder.Services.AddInfrastructureServices(builder.Configuration);

// Configura autenticação por cookies nativa do Identity
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.AccessDeniedPath = "/Account/AccessDenied";
    options.ExpireTimeSpan = TimeSpan.FromHours(8);
    options.SlidingExpiration = true;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// OBRIGATÓRIO: Authentication ANTES de Authorization
app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();

app.Run();

