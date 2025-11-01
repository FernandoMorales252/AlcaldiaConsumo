using AlcaldiaFront.Services;
using AlcaldiaFront.WebApp.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Defincion de la url base de la Api
builder.Services.AddHttpClient<ApiService>(client =>
{
    

    client.BaseAddress = new Uri("https://localhost:7053/api/"); 
});

//Servicios
builder.Services.AddScoped<AlcaldiaFront.Services.TipoDocService>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<AlcaldiaFront.Services.MunicipioService>();
builder.Services.AddScoped<AlcaldiaFront.Services.DocumentoService>();
builder.Services.AddScoped<AlcaldiaFront.Services.CargoService>();
builder.Services.AddScoped<AlcaldiaFront.Services.InventarioService>();
builder.Services.AddScoped<AlcaldiaFront.Services.ProyectoService>();
builder.Services.AddScoped<AlcaldiaFront.Services.EmpleadoService>();
builder.Services.AddScoped<AlcaldiaFront.Services.AvisoService>();
builder.Services.AddScoped<AlcaldiaFront.Services.QuejaService>();

// Configuraci�n de la autenticaci�n de la aplicaci�n usando cookies
builder.Services.AddAuthentication("AuthCookie")
    .AddCookie("AuthCookie", options =>
    {
        options.LoginPath = "/Auth/Login";
        options.ExpireTimeSpan = TimeSpan.FromMinutes(60); // Despu�s de 60 minutos, el usuario tendr� que iniciar sesi�n nuevamente
    });

var app = builder.Build();



// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();


app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
