using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using PatsyLibrary.Contracts.DataAccess;
using PatsyLibrary.Contracts.DataAccess.Interfaces;
using PatsyLibrary.Contracts.Services;
using PatsyLibrary.Contracts.Services.Interfaces;
using PatsyLibrary.Data;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Configurar o DbContext com retentativas
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"), npgsqlOptions =>
    {
        npgsqlOptions.EnableRetryOnFailure(
            maxRetryCount: 5, // Número máximo de tentativas
            maxRetryDelay: TimeSpan.FromSeconds(10), // Atraso máximo entre tentativas
            errorCodesToAdd: null // Usa os erros transitórios padrão do Npgsql
        );
    });
});

builder.Services.AddScoped<IUnitOfWorkRepository, UnitOfWorkRepository>();
builder.Services.AddScoped<IBookService, BookService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ISeedUserRoleInitial, SeedUserRoleInitial>();

builder.Services.AddHttpContextAccessor();
builder.Services.AddSession(options =>
{
    options.Cookie.HttpOnly = true;  // Impede acesso via JavaScript
    options.Cookie.IsEssential = true;  // Marca a cookie como essencial
    options.IdleTimeout = TimeSpan.FromSeconds(100); // Define o tempo de expiração da sessão
});

// Configurar autenticação com cookies
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
    {
        options.LoginPath = "/Account/Login"; // Ajuste para sua rota de login
        options.LogoutPath = "/Account/Logout"; // Ajuste para sua rota de logout
        options.AccessDeniedPath = "/Account/AccessDenied"; // Ajuste para acesso negado
        options.ExpireTimeSpan = TimeSpan.FromSeconds(10); // Tempo de expiração do cookie
    });

// Add services to the container.
builder.Services.AddControllersWithViews();


WebApplication app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

// Habilitando o uso da sessão
app.UseSession();

app.UseRouting();

// Adicionar autenticação, autorização e sessão ao pipeline
app.UseAuthentication();
app.UseAuthorization();

CriarPerfisUsuarios(app).GetAwaiter().GetResult();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();


async Task CriarPerfisUsuarios(WebApplication app)
{
    IServiceScopeFactory scopedFactory = app.Services.GetService<IServiceScopeFactory>();
    using IServiceScope scope = scopedFactory.CreateScope();
    ISeedUserRoleInitial service = scope.ServiceProvider.GetService<ISeedUserRoleInitial>();
    await service.SeedUsers();
}