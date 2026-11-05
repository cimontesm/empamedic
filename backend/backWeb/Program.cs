using backWeb.Data;
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
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();
app.MapHealthChecks("/health");

app.Run();

// Permite que el proyecto de pruebas levante la API con WebApplicationFactory<Program>.
public partial class Program
{
}
