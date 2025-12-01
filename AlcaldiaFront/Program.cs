using System.Net.Http.Headers;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using AlcaldiaFront.Services;
using AlcaldiaFront.WebApp.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddHttpContextAccessor();

builder.Services.AddHttpClient("Api", c =>
{
    var baseUrl = builder.Configuration["ApiBaseUrl"];
    if (string.IsNullOrWhiteSpace(baseUrl))
        throw new InvalidOperationException("Config 'ApiBaseUrl' no está definida en appsettings.json.");

    c.BaseAddress = new Uri(baseUrl);
    c.DefaultRequestHeaders.Accept.Add(
        new MediaTypeWithQualityHeaderValue("application/json"));
});

builder.Services.AddScoped<ApiService>(sp =>
{
    var factory = sp.GetRequiredService<IHttpClientFactory>();
    var httpClient = factory.CreateClient("Api");
    return new ApiService(httpClient);
});

builder.Services.AddScoped<TipoDocService>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<MunicipioService>();
builder.Services.AddScoped<DocumentoService>();
builder.Services.AddScoped<CargoService>();
builder.Services.AddScoped<InventarioService>();
builder.Services.AddScoped<ProyectoService>();
builder.Services.AddScoped<EmpleadoService>();
builder.Services.AddScoped<AvisoService>();
builder.Services.AddScoped<QuejaService>();
builder.Services.AddScoped<DashboardService>();


builder.Services.AddAuthentication("AuthCookie")
    .AddCookie("AuthCookie", opts =>
    {
        opts.LoginPath = "/Auth/Login";
        opts.LogoutPath = "/Auth/Logout";
        opts.AccessDeniedPath = "/Auth/Denied";
        opts.Cookie.Name = ".AlcaldiaFront.Auth";
        opts.Cookie.HttpOnly = true;

        opts.Cookie.SameSite = SameSiteMode.Lax;
        opts.Cookie.SecurePolicy = CookieSecurePolicy.None;

        opts.SlidingExpiration = true;
        opts.ExpireTimeSpan = TimeSpan.FromMinutes(60);
    });

// ===== Autorización =====
builder.Services.AddAuthorization();


var app = builder.Build();

// ===== Pipeline =====
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

// Si la API es HTTP, el Frontend NO debe forzar HTTPS internamente.
// app.UseHttpsRedirection(); 

app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();