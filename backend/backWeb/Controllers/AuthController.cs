using backWeb.Data;
using backWeb.DTOs;
using backWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace backWeb.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _context;

    public AuthController(AppDbContext context)
    {
        _context = context;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto request)
    {
        if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
        {
            return BadRequest(new { mensaje = "El usuario y la contraseña son requeridos." });
        }

        // Búsqueda del usuario activo por username o email
        var usuario = await _context.Usuarios
            .FirstOrDefaultAsync(u => (u.Username == request.Username || u.Email == request.Username) && u.Activo);

        if (usuario == null)
        {
            return Unauthorized(new { mensaje = "Credenciales incorrectas o usuario inactivo." });
        }

        // Verificación básica de contraseña (cumplimiento RF01)
        if (usuario.PasswordHash != request.Password)
        {
            return Unauthorized(new { mensaje = "Credenciales incorrectas." });
        }

        return Ok(new
        {
            mensaje = "Inicio de sesión exitoso",
            idUsuario = usuario.IdUsuario,
            nombre = $"{usuario.Nombre} {usuario.Apellido}",
            rol = usuario.Rol.ToString(),
            esMedico = usuario.EsMedico()
        });
    }

    [HttpPost("registro")]
    public async Task<IActionResult> Registro([FromBody] Usuario nuevoUsuario)
    {
        var existe = await _context.Usuarios
            .AnyAsync(u => u.Username == nuevoUsuario.Username || u.Email == nuevoUsuario.Email);

        if (existe)
        {
            return BadRequest(new { mensaje = "El nombre de usuario o email ya está en uso." });
        }

        _context.Usuarios.Add(nuevoUsuario);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(Login), new { id = nuevoUsuario.IdUsuario }, new { mensaje = "Usuario registrado con éxito." });
    }
}
