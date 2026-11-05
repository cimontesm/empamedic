namespace appEmpamedic.Services
{
    /// <summary>
    /// Datos simulados en memoria para desarrollar la app sin depender del backend.
    /// Todos los nombres y datos son ficticios.
    /// </summary>
    public class MockEmpamedicApi : IEmpamedicApi
    {
        private readonly List<Colaborador> _colaboradores;
        private readonly List<ConsultaMedica> _consultas;
        private readonly List<Medicamento> _medicamentos;
        private readonly List<SeguimientoAlerta> _alertas;

        public MockEmpamedicApi()
        {
            var hoy = DateTime.Today;

            _colaboradores =
            [
                new() { Id = 1, Cedula = "0900000001", Nombres = "María", Apellidos = "Pérez Loor", Cargo = "Operaria de empaque", Area = "Empaque", FechaNacimiento = new(1990, 3, 12), TipoSangre = "O+", Alergias = "Penicilina" },
                new() { Id = 2, Cedula = "0900000002", Nombres = "José", Apellidos = "Rodríguez Vera", Cargo = "Montacarguista", Area = "Bodega", FechaNacimiento = new(1985, 7, 2), TipoSangre = "A+", Antecedentes = "Hipertensión" },
                new() { Id = 3, Cedula = "0900000003", Nombres = "Ana", Apellidos = "Castro Mendoza", Cargo = "Clasificadora", Area = "Proceso", FechaNacimiento = new(1998, 11, 20), TipoSangre = "B+" },
                new() { Id = 4, Cedula = "0900000004", Nombres = "Luis", Apellidos = "Zambrano Ruiz", Cargo = "Técnico de frío", Area = "Mantenimiento", FechaNacimiento = new(1979, 5, 8), TipoSangre = "O-", Antecedentes = "Diabetes tipo 2" },
            ];

            _consultas =
            [
                new() { Id = 1, ColaboradorId = 1, NombreColaborador = "María Pérez Loor", Fecha = hoy.AddDays(-10), MotivoConsulta = "Dolor lumbar", Diagnostico = "Lumbalgia mecánica", Tratamiento = "Ibuprofeno 400 mg c/8h por 5 días", FechaProximoControl = hoy.AddDays(2) },
                new() { Id = 2, ColaboradorId = 2, NombreColaborador = "José Rodríguez Vera", Fecha = hoy.AddDays(-30), MotivoConsulta = "Control de presión", Diagnostico = "Hipertensión controlada", Tratamiento = "Losartán 50 mg diario", FechaProximoControl = hoy.AddDays(-1) },
                new() { Id = 3, ColaboradorId = 4, NombreColaborador = "Luis Zambrano Ruiz", Fecha = hoy.AddDays(-5), MotivoConsulta = "Control de glucosa", Diagnostico = "Diabetes tipo 2", Tratamiento = "Metformina 850 mg", FechaProximoControl = hoy.AddDays(5) },
                new() { Id = 4, ColaboradorId = 3, NombreColaborador = "Ana Castro Mendoza", Fecha = hoy, MotivoConsulta = "Corte en mano", Diagnostico = "Herida superficial", Tratamiento = "Curación y antiséptico" },
            ];

            _medicamentos =
            [
                new() { Id = 1, Nombre = "Ibuprofeno", Presentacion = "Tableta 400 mg", StockActual = 120, StockMinimo = 30 },
                new() { Id = 2, Nombre = "Paracetamol", Presentacion = "Tableta 500 mg", StockActual = 25, StockMinimo = 40 },
                new() { Id = 3, Nombre = "Losartán", Presentacion = "Tableta 50 mg", StockActual = 60, StockMinimo = 20 },
                new() { Id = 4, Nombre = "Metformina", Presentacion = "Tableta 850 mg", StockActual = 10, StockMinimo = 15 },
            ];

            // Médico 1 es el usuario simulado de la sesión; la alerta del médico 2 no debe verse.
            _alertas =
            [
                new() { Id = 1, ColaboradorId = 2, NombreColaborador = "José Rodríguez Vera", MedicoResponsableId = 1, ConsultaMedicaId = 2, FechaControl = hoy.AddDays(-1), Motivo = "Control de presión arterial" },
                new() { Id = 2, ColaboradorId = 1, NombreColaborador = "María Pérez Loor", MedicoResponsableId = 1, ConsultaMedicaId = 1, FechaControl = hoy.AddDays(2), Motivo = "Revisión de lumbalgia" },
                new() { Id = 3, ColaboradorId = 4, NombreColaborador = "Luis Zambrano Ruiz", MedicoResponsableId = 2, ConsultaMedicaId = 3, FechaControl = hoy.AddDays(5), Motivo = "Control de glucosa" },
            ];
        }

        public Task<List<Colaborador>> ObtenerColaboradoresAsync()
            => Task.FromResult(_colaboradores.OrderBy(c => c.Apellidos).ToList());

        public Task<Colaborador?> ObtenerColaboradorAsync(int id)
            => Task.FromResult(_colaboradores.FirstOrDefault(c => c.Id == id));

        public Task<List<ConsultaMedica>> ObtenerConsultasAsync(int? colaboradorId = null)
            => Task.FromResult(_consultas
                .Where(c => colaboradorId is null || c.ColaboradorId == colaboradorId)
                .OrderByDescending(c => c.Fecha)
                .ToList());

        public Task<List<Medicamento>> ObtenerMedicamentosAsync()
            => Task.FromResult(_medicamentos.OrderBy(m => m.Nombre).ToList());

        public Task<List<SeguimientoAlerta>> ObtenerAlertasPendientesAsync(int medicoId)
            => Task.FromResult(_alertas
                .Where(a => a.MedicoResponsableId == medicoId && a.Estado == EstadoAlerta.Pendiente)
                .OrderBy(a => a.FechaControl)
                .ToList());

        public Task ActualizarEstadoAlertaAsync(int alertaId, EstadoAlerta estado)
        {
            var alerta = _alertas.FirstOrDefault(a => a.Id == alertaId);
            if (alerta is not null)
            {
                alerta.Estado = estado;
                alerta.FechaResolucion = estado == EstadoAlerta.Pendiente ? null : DateTime.Now;
            }
            return Task.CompletedTask;
        }
    }
}
