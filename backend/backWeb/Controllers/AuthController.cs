using backWeb.Data;
using backWeb.DTOs;
using backWeb.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace backWeb.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly PasswordHasher<Usuario> _hasher = new();

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

        // Verificación de contraseña:
        // 1. Intenta verificar con hash PBKDF2 (RNF03)
        // 2. Soporte de contingencia: si la contraseña en BD aún estaba en texto plano, valida y actualiza al nuevo hash
        var resultadoHash = _hasher.VerifyHashedPassword(usuario, usuario.PasswordHash, request.Password);

        if (resultadoHash == PasswordVerificationResult.Failed)
        {
            if (usuario.PasswordHash == request.Password)
            {
                usuario.PasswordHash = _hasher.HashPassword(usuario, request.Password);
                await _context.SaveChangesAsync();
            }
            else
            {
                return Unauthorized(new { mensaje = "Credenciales incorrectas." });
            }
        }
        else if (resultadoHash == PasswordVerificationResult.SuccessRehashNeeded)
        {
            usuario.PasswordHash = _hasher.HashPassword(usuario, request.Password);
            await _context.SaveChangesAsync();
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

        // Cumplimiento RNF03: Hasheado criptográfico antes de almacenar en base de datos
        nuevoUsuario.PasswordHash = _hasher.HashPassword(nuevoUsuario, nuevoUsuario.PasswordHash);

        _context.Usuarios.Add(nuevoUsuario);
        await _context.SaveChangesAsync();

        return Ok(new
        {
            mensaje = "Usuario registrado con éxito.",
            idUsuario = nuevoUsuario.IdUsuario
        });
    }
}
