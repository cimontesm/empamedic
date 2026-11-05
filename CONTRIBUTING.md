# Cómo trabajamos en EMPAMEDIC

## Branches

- `main` es la versión estable. **Nadie hace push directo a `main`**: todo entra por Pull Request.
- Cada integrante trabaja en su propio branch (por ejemplo `bryan_AppMovil`).
- Antes de abrir un PR, actualiza tu branch con lo último de `main`:

  ```bash
  git fetch origin
  git merge origin/main
  ```

## Pull Requests

1. Abre el PR de tu branch hacia `main` y llena la plantilla.
2. El CI (GitHub Actions) debe pasar: compila backend, frontend y app, ejecuta las pruebas y revisa el formato.
3. Al menos **un compañero** revisa y aprueba el PR antes del merge.
4. PRs pequeños y frecuentes: es más fácil revisarlos y generan menos conflictos.

## Base de datos (EF Core)

- Hay **un solo** `AppDbContext` en `backend/backWeb/Data/AppDbContext.cs`. Agrega ahí tus `DbSet`.
- La configuración de cada entidad va en su propia clase `IEntityTypeConfiguration<T>` dentro de `Data/Configurations/`.
- Después de cambiar el modelo, crea la migración y súbela en el mismo PR:

  ```bash
  dotnet tool restore
  dotnet ef migrations add NombreDelCambio --project backend/backWeb
  ```

- Si dos PRs traen migraciones a la vez, el segundo en hacer merge debe actualizarse con `main` y volver a generar su migración.

## Estilo de código

- El archivo `.editorconfig` define el estilo; Visual Studio lo aplica solo.
- Antes de hacer commit puedes formatear con:

  ```bash
  dotnet format backend/backWeb/backWeb.csproj
  ```

- Nombres en español para el dominio (`Colaborador`, `ConsultaMedica`), en PascalCase. Interfaces con prefijo `I`. Campos privados con `_`.

## Secretos

- **Nunca** subas contraseñas, cadenas de conexión con usuario/clave, claves de Firebase ni `google-services.json`.
- Usa `dotnet user-secrets` (ver README).
