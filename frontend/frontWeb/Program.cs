using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

// Servicios para MVC
builder.Services.AddControllersWithViews();

// 1. Cliente HTTP para consumir los endpoints de la API (backWeb)
builder.Services.AddHttpClient("BackendApi", client =>
{
    var apiBaseUrl = builder.Configuration["BackendApi:BaseUrl"] ?? "https://localhost:7150";
    client.BaseAddress = new Uri(apiBaseUrl);
});

// 2. Autenticacion por Cookies con expiracion por inactividad a los 10 minutos (RNF06)
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.LogoutPath = "/Account/Logout";
        options.AccessDeniedPath = "/Account/AccessDenied";
        options.ExpireTimeSpan = TimeSpan.FromMinutes(10); // RNF06: 10 minutos de inactividad
        options.SlidingExpiration = true; // Renueva el tiempo mientras el usuario este activo
    });

// 3. Manejo de estado de sesion
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(10); // RNF06
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Habilitar sesiones y autenticacion antes de la autorizacion
app.UseSession();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");

app.Run();
