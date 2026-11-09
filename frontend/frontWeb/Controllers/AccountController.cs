using System.Security.Claims;
using System.Text;
using System.Text.Json;
using frontWeb.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;

namespace frontWeb.Controllers;

public class AccountController : Controller
{
    private readonly IHttpClientFactory _httpClientFactory;

    public AccountController(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    [HttpGet]
    public IActionResult Login(string? returnUrl = null)
    {
        if (User.Identity?.IsAuthenticated == true)
        {
            return RedirectToAction("Index", "Home");
        }

        ViewData["ReturnUrl"] = returnUrl;
        return View(new LoginViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var client = _httpClientFactory.CreateClient("BackendApi");
        var payload = new
        {
            Username = model.Correo,
            Password = model.Password
        };

        var jsonContent = new StringContent(
            JsonSerializer.Serialize(payload),
            Encoding.UTF8,
            "application/json"
        );

        HttpResponseMessage response;
        try
        {
            response = await client.PostAsync("/api/auth/login", jsonContent);
        }
        catch (Exception)
        {
            ModelState.AddModelError(string.Empty, "No fue posible conectar con el servicio de autenticación. Intente más tarde.");
            return View(model);
        }

        if (!response.IsSuccessStatusCode)
        {
            ModelState.AddModelError(string.Empty, "Credenciales incorrectas o usuario inactivo.");
            return View(model);
        }

        var responseBody = await response.Content.ReadAsStringAsync();
        using var document = JsonDocument.Parse(responseBody);
        var root = document.RootElement;

        var rol = root.TryGetProperty("rol", out var rolProp) ? rolProp.GetString() : "Usuario";
        var nombre = root.TryGetProperty("nombre", out var nomProp) ? nomProp.GetString() : model.Correo;

        var claims = new List<Claim>
        {
            new(ClaimTypes.Name, nombre ?? model.Correo),
            new(ClaimTypes.Email, model.Correo),
            new(ClaimTypes.Role, rol ?? "Usuario")
        };

        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var authProperties = new AuthenticationProperties
        {
            IsPersistent = false,
            ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(10) // RNF06
        };

        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(claimsIdentity),
            authProperties
        );

        if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
        {
            return Redirect(returnUrl);
        }

        return RedirectToAction("Index", "Home");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult ExtenderSesion()
    {
        // Al acceder a esta accion estando autenticado con SlidingExpiration activa,
        // el middleware de CookieAuthentication renueva automaticamente la ventana de expiracion
        return Ok(new { mensaje = "Sesión renovada con éxito." });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        HttpContext.Session.Clear();
        return RedirectToAction("Login", "Account");
    }

    [HttpGet]
    public IActionResult AccessDenied()
    {
        return View();
    }
}
