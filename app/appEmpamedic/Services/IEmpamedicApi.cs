namespace appEmpamedic.Services
{
    /// <summary>
    /// Acceso a los datos del backend (backWeb).
    /// Por ahora se usa <see cref="MockEmpamedicApi"/>; cuando existan los endpoints
    /// se agrega una implementación con HttpClient y se cambia el registro en MauiProgram.
    /// </summary>
    public interface IEmpamedicApi
    {
        Task<List<Colaborador>> ObtenerColaboradoresAsync();
        Task<Colaborador?> ObtenerColaboradorAsync(int id);

        Task<List<ConsultaMedica>> ObtenerConsultasAsync(int? colaboradorId = null);

        Task<List<Medicamento>> ObtenerMedicamentosAsync();

        /// <summary>Alertas pendientes del médico indicado (solo las de sus pacientes).</summary>
        Task<List<SeguimientoAlerta>> ObtenerAlertasPendientesAsync(int medicoId);
        Task ActualizarEstadoAlertaAsync(int alertaId, EstadoAlerta estado);
    }
}
