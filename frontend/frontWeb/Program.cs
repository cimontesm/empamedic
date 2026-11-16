using frontWeb.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// --- Cliente HTTP hacia el backend (backWeb). El frontend MVC no toca ninguna
// base de datos directamente: todos los datos pasan por este cliente. ---
builder.Services.AddHttpClient<IReportesApiClient, ReportesApiClient>(client =>
{
    var baseUrl = builder.Configuration["BackendApi:BaseUrl"]
        ?? throw new InvalidOperationException("Falta configurar 'BackendApi:BaseUrl' en appsettings.json");
    client.BaseAddress = new Uri(baseUrl);
    client.Timeout = TimeSpan.FromSeconds(10);
}); var app = builder.Build();

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

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
