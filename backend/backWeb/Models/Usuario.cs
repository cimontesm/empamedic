using System.ComponentModel.DataAnnotations;

namespace backWeb.Models;

public class Usuario
{
    [Key]
    public int IdUsuario { get; set; }

    public RolUsuario Rol { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Apellido { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public bool Activo { get; set; } = true;

    public bool Autenticar(string password)
    {
        return Activo;
    }

    public bool EsMedico()
    {
        return Rol == RolUsuario.MEDICO;
    }
}
