using backWeb.Data;
using backWeb.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Base de datos: la cadena de conexión "EmpamedicDb" viene de appsettings.Development.json
// o de "dotnet user-secrets" (ver README). Nunca se guardan contraseñas en el repositorio.
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("EmpamedicDb")));

builder.Services.AddControllers();
builder.Services.AddHealthChecks();

// Swagger/OpenAPI: https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

    // Aplica migraciones pendientes y crea usuario inicial de prueba si hay conexión a BD
    try
    {
        using var scope = app.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        if (context.Database.IsRelational() && !string.IsNullOrEmpty(context.Database.GetConnectionString()))
        {
            context.Database.Migrate();

            if (!context.Usuarios.Any())
            {
                var hasher = new PasswordHasher<Usuario>();
                var admin = new Usuario
                {
                    Nombre = "Mateo",
                    Apellido = "Zambrano",
                    Username = "mateo.zambrano@empamedic.com",
                    Email = "mateo.zambrano@empamedic.com",
                    Rol = RolUsuario.ADMINISTRADOR,
                    Activo = true
                };
                admin.PasswordHash = hasher.HashPassword(admin, "Admin123*");

                context.Usuarios.Add(admin);
                context.SaveChanges();
            }
        }
    }
    catch (Exception ex)
    {
        // Evita que fallen las pruebas de integración (CI/CD) cuando no hay instancia de SQL Server
        app.Logger.LogWarning(ex, "Omitiendo migración/seed automático porque no hay base de datos disponible.");
    }
}
else
{
    // En desarrollo usamos HTTP (5164) para evitar bloqueos de certificados SSL locales entre front y back
    app.UseHttpsRedirection();
}

app.UseAuthorization();
app.MapControllers();
app.MapHealthChecks("/health");

app.Run();

// Permite que el proyecto de pruebas levante la API con WebApplicationFactory<Program>.
public partial class Program
{
}
