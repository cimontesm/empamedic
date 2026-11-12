# EMPAMEDIC

Sistema de gestión del dispensario médico de EMPACRECI S.A.: pacientes (colaboradores), historial clínico, consultas, inventario de medicamentos, alertas de control y reportes.

Proyecto de Ingeniería de Software I — ESPOL, 2026 PAO II.

## Estructura

| Carpeta | Proyecto | Tecnología |
|---|---|---|
| `backend/backWeb` | API REST | ASP.NET Core Web API, .NET 8, EF Core 8, SQL Server 2019 |
| `backend/backWeb.Tests` | Pruebas del backend | xUnit |
| `frontend/frontWeb` | Plataforma web | ASP.NET Core MVC, .NET 8 |
| `app/appEmpamedic` | App móvil | .NET MAUI, .NET 9 |

Abre todo junto con `empamedic.sln` en la raíz.

Prototipo de pantallas: <https://lovable.dev/projects/ee800fd2-bac2-43be-b1e7-7feacb113641>

## Requisitos

- Visual Studio 2022 (17.12+) o 2026, con las cargas de trabajo **ASP.NET y desarrollo web** y **Desarrollo de interfaz de usuario de aplicaciones multiplataforma de .NET (MAUI)**.
- **.NET 8 Runtime/SDK** (para ejecutar el backend y el frontend) y SDK de .NET 9 o superior (para la app).
- SQL Server 2019 (Developer o Express) y, opcionalmente, SQL Server Management Studio.

## Primeros pasos

```bash
git clone https://github.com/cimontesm/empamedic.git
cd empamedic
dotnet tool restore
```

### Base de datos

Por defecto el backend se conecta a `localhost` con autenticación de Windows (ver `backend/backWeb/appsettings.Development.json`).

Si tu SQL Server usa otro servidor o usuario y contraseña, **no edites el archivo**: guarda tu cadena en user-secrets, que queda solo en tu equipo:

```bash
dotnet user-secrets set "ConnectionStrings:EmpamedicDb" "Server=localhost\SQLEXPRESS;Database=Empamedic;User Id=sa;Password=TU_CLAVE;TrustServerCertificate=True" --project backend/backWeb
```

Crear o actualizar la base con las migraciones:

```bash
dotnet ef database update --project backend/backWeb
```

### Ejecutar

```bash
dotnet run --project backend/backWeb
```

- Swagger: <https://localhost:7237/swagger>
- Estado: <https://localhost:7237/health>

```bash
dotnet run --project frontend/frontWeb
```

La app móvil se ejecuta desde Visual Studio eligiendo un emulador Android o Windows Machine.

### Pruebas

```bash
dotnet test backend/backWeb.Tests
```

## Cómo contribuir

Lee [CONTRIBUTING.md](CONTRIBUTING.md): branches, Pull Requests, migraciones y estilo de código.
