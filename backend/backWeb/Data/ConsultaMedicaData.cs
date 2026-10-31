// Esto es temporal para poder simular consultas y hacer los reportes.


using backWeb.Models;

namespace backWeb.Data;

public static class ConsultaMedicaData
{
    public static readonly List<ConsultaMedica> Consultas = new()
    {
        new() { Id = 1, PacienteId = "EMP001", NombrePaciente  = "Carlos Mendoza", FechaConsulta  = new DateTime(2026, 10, 2), Diagnostico = "Gripe", Tratamiento = "Neogripal", DiasReposo = 2 },
        new() { Id = 2, PacienteId = "EMP002", NombrePaciente  = "Ana Torres", FechaConsulta  = new DateTime(2026, 10, 3), Diagnostico = "Migraña", Tratamiento = "Umbral", DiasReposo = 1 },
        new() { Id = 3, PacienteId = "EMP003", NombrePaciente  = "Luis Vera", FechaConsulta  = new DateTime(2026, 10, 5), Diagnostico = "Gripe", Tratamiento = "Neogripal", DiasReposo = 3 },
        new() { Id = 4, PacienteId = "EMP001", NombrePaciente  = "Carlos Mendoza", FechaConsulta  = new DateTime(2026, 10, 8), Diagnostico = "Gastritis", Tratamiento = "Digeril", DiasReposo = 0 },
        new() { Id = 5, PacienteId = "EMP004", NombrePaciente  = "María López", FechaConsulta  = new DateTime(2026, 10, 10), Diagnostico = "Migraña", Tratamiento = "Umbral", DiasReposo = 1 },
        new() { Id = 6, PacienteId = "EMP005", NombrePaciente  = "Pedro Ruiz", FechaConsulta  = new DateTime(2026, 10, 12), Diagnostico = "Resfriado", Tratamiento = "Neogripal", DiasReposo = 2 },
        new() { Id = 7, PacienteId = "EMP002", NombrePaciente  = "Ana Torres", FechaConsulta  = new DateTime(2026, 10, 15), Diagnostico = "Gripe", Tratamiento = "Neogripal", DiasReposo = 2 },
        new() { Id = 8, PacienteId = "EMP006", NombrePaciente  = "Sofía Castro", FechaConsulta  = new DateTime(2026, 10, 18), Diagnostico = "Gastritis", Tratamiento = "Digeril", DiasReposo = 0 },
        new() { Id = 9, PacienteId = "EMP007", NombrePaciente  = "Diego Ruiz", FechaConsulta  = new DateTime(2026, 10, 20), Diagnostico = "Resfriado", Tratamiento = "Neogripal", DiasReposo = 1 },
        new() { Id = 10, PacienteId = "EMP003", NombrePaciente  = "Luis Vera", FechaConsulta  = new DateTime(2026, 10, 25), Diagnostico = "Gripe", Tratamiento = "Neogripal", DiasReposo = 2 }
    };
}
