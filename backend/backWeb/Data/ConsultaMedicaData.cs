// Esto es temporal para poder simular consultas y hacer los reportes.


using backWeb.Models;

namespace backWeb.Data;

public static class ConsultaMedicaData
{
    public static readonly List<ConsultaMedica> Consultas = new()
    {
        new() { Id = 1, CedulaPaciente = "EMP001", NombrePaciente = "Carlos Mendoza", FechaConsulta = new DateTime(2026, 11, 2), MotivoConsulta = "Dolor de cabeza y fiebre", Sintomas = "Fiebre, malestar general", Diagnostico = "Gripe", Observaciones = null, PresionArterial = "120/80", Temperatura = 38.2m, Tratamiento = "Reposo e hidratación", MedicinaRecetada = "Neogripal", DiasReposo = 2 },
        new() { Id = 2, CedulaPaciente = "EMP002", NombrePaciente = "Ana Torres", FechaConsulta = new DateTime(2026, 11, 3), MotivoConsulta = "Dolor de cabeza intenso", Sintomas = "Dolor pulsátil, sensibilidad a la luz", Diagnostico = "Migraña", Observaciones = "Antecedente de migrañas recurrentes", PresionArterial = "110/70", Temperatura = null, Tratamiento = "Analgésico y descanso en lugar oscuro", MedicinaRecetada = "Umbral", DiasReposo = 1 },
        new() { Id = 3, CedulaPaciente = "EMP003", NombrePaciente = "Luis Vera", FechaConsulta = new DateTime(2026, 11, 5), MotivoConsulta = "Congestión nasal", Sintomas = "Tos, congestión", Diagnostico = "Gripe", Observaciones = null, PresionArterial = null, Temperatura = 37.8m, Tratamiento = "Reposo", MedicinaRecetada = "Neogripal", DiasReposo = 3 },
        new() { Id = 4, CedulaPaciente = "EMP001", NombrePaciente = "Carlos Mendoza", FechaConsulta = new DateTime(2026, 11, 8), MotivoConsulta = "Dolor abdominal", Sintomas = "Ardor estomacal", Diagnostico = "Gastritis", Observaciones = "Recomendado evitar comida irritante", PresionArterial = "118/76", Temperatura = null, Tratamiento = "Dieta blanda", MedicinaRecetada = "Digeril", DiasReposo = 0 },
        new() { Id = 5, CedulaPaciente = "EMP004", NombrePaciente = "María López", FechaConsulta = new DateTime(2026, 11, 9), MotivoConsulta = "Dolor de cabeza", Sintomas = "Náusea leve", Diagnostico = "Migraña", Observaciones = null, PresionArterial = "115/75", Temperatura = null, Tratamiento = "Analgésico", MedicinaRecetada = "Umbral", DiasReposo = 1 },
        new() { Id = 6, CedulaPaciente = "EMP005", NombrePaciente = "Pedro Ruiz", FechaConsulta = new DateTime(2026, 11, 9), MotivoConsulta = "Malestar general", Sintomas = "Estornudos, congestión leve", Diagnostico = "Resfriado", Observaciones = null, PresionArterial = null, Temperatura = 37.1m, Tratamiento = "Reposo e hidratación", MedicinaRecetada = "Neogripal", DiasReposo = 2 },
        new() { Id = 7, CedulaPaciente = "EMP002", NombrePaciente = "Ana Torres", FechaConsulta = new DateTime(2026, 11, 10), MotivoConsulta = "Fiebre y tos", Sintomas = "Fiebre, tos seca", Diagnostico = "Gripe", Observaciones = null, PresionArterial = "112/72", Temperatura = 38.5m, Tratamiento = "Reposo", MedicinaRecetada = "Neogripal", DiasReposo = 2 },
        new() { Id = 8, CedulaPaciente = "EMP006", NombrePaciente = "Sofía Castro", FechaConsulta = new DateTime(2026, 11, 10), MotivoConsulta = "Dolor abdominal leve", Sintomas = "Ardor estomacal ocasional", Diagnostico = "Gastritis", Observaciones = null, PresionArterial = "120/78", Temperatura = null, Tratamiento = "Dieta blanda", MedicinaRecetada = "Digeril", DiasReposo = 0 },
        new() { Id = 9, CedulaPaciente = "EMP007", NombrePaciente = "Diego Ruiz", FechaConsulta = new DateTime(2026, 11, 11), MotivoConsulta = "Congestión y estornudos", Sintomas = "Congestión nasal", Diagnostico = "Resfriado", Observaciones = null, PresionArterial = null, Temperatura = null, Tratamiento = "Reposo", MedicinaRecetada = "Neogripal", DiasReposo = 1 },
        new() { Id = 10, CedulaPaciente = "EMP003", NombrePaciente = "Luis Vera", FechaConsulta = new DateTime(2026, 11, 11), MotivoConsulta = "Control de seguimiento", Sintomas = null, Diagnostico = "Control", Observaciones = "Evolución favorable, sin medicación adicional", PresionArterial = "118/74", Temperatura = null, Tratamiento = "Ninguno, seguimiento normal", MedicinaRecetada = "", DiasReposo = 0 },
        new() { Id = 11, CedulaPaciente = "EMP008", NombrePaciente = "Valeria Gómez", FechaConsulta = new DateTime(2026, 12, 1), MotivoConsulta = "Dolor de cabeza y fiebre", Sintomas = "Fiebre alta, dolor de cabeza intenso", Diagnostico = "Gripe", Observaciones = null, PresionArterial = "110/70", Temperatura = 39.0m, Tratamiento = "Reposo e hidratación", MedicinaRecetada = "Neogripal", DiasReposo = 3 },
        new() { Id = 12, CedulaPaciente = "EMP009", NombrePaciente = "Javier Morales", FechaConsulta = new DateTime(2026, 12, 2), MotivoConsulta = "Dolor abdominal y náuseas", Sintomas = "Náuseas, dolor abdominal leve", Diagnostico = "Gastritis", Observaciones = null, PresionArterial = "115/75", Temperatura = null, Tratamiento = "Dieta blanda y reposo", MedicinaRecetada = "Digeril", DiasReposo = 1 },
    };
}
