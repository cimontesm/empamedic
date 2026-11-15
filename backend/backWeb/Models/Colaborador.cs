using System.ComponentModel.DataAnnotations;

namespace backWeb.Models;

public class Colaborador
{
    public int IdColaborador { get; set; }

    public string Cedula { get; set; }
    public string Nombres { get; set; }
    public string Apellidos { get; set; }
    public DateTime FechaNacimiento { get; set; }
    public string Genero { get; set; }
    public string Cargo { get; set; }
    public string AreaTrabajo { get; set; }
    public string TipoSangre { get; set; }
    public string Telefono { get; set; }
    public string Alergias { get; set; }
    public string AntecedentesPatologicos { get; set; }
    public string MedicacionContinua { get; set; }

    public List<ConsultaMedica> HistorialMedico { get; set; } = new List<ConsultaMedica>();

    public string ObtenerNombreCompleto()
    {
        return $"{Nombres} {Apellidos}";
    }

    public int CalcularEdad()
    {
        var hoy = DateTime.Today;
        int edad = hoy.Year - FechaNacimiento.Year;
        if (FechaNacimiento.Date > hoy.AddYears(-edad)) edad--;
        return edad;
    }

    public void ActualizarAntecedentes(string nuevasAlergias, string nuevosAntecedentes)
    {
        Alergias = nuevasAlergias;
        AntecedentesPatologicos = nuevosAntecedentes;
    }
}
