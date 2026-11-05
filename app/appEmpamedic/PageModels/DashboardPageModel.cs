using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace appEmpamedic.PageModels
{
    public partial class DashboardPageModel : ObservableObject
    {
        private readonly IEmpamedicApi _api;
        private readonly SesionService _sesion;
        private readonly IErrorHandler _errorHandler;

        [ObservableProperty]
        private string _saludo = string.Empty;

        [ObservableProperty]
        private int _totalPacientes;

        [ObservableProperty]
        private int _consultasHoy;

        [ObservableProperty]
        private int _medicamentosStockBajo;

        [ObservableProperty]
        private bool _isRefreshing;

        [ObservableProperty]
        private bool _sinAlertas;

        public ObservableCollection<SeguimientoAlerta> AlertasPendientes { get; } = [];

        public DashboardPageModel(IEmpamedicApi api, SesionService sesion, IErrorHandler errorHandler)
        {
            _api = api;
            _sesion = sesion;
            _errorHandler = errorHandler;
        }

        [RelayCommand]
        public async Task CargarAsync()
        {
            if (_sesion.UsuarioId is not int medicoId)
                return;

            try
            {
                IsRefreshing = true;
                Saludo = $"Hola, {_sesion.NombreUsuario}";

                var pacientes = await _api.ObtenerColaboradoresAsync();
                var consultas = await _api.ObtenerConsultasAsync();
                var medicamentos = await _api.ObtenerMedicamentosAsync();
                var alertas = await _api.ObtenerAlertasPendientesAsync(medicoId);

                TotalPacientes = pacientes.Count;
                ConsultasHoy = consultas.Count(c => c.Fecha.Date == DateTime.Today);
                MedicamentosStockBajo = medicamentos.Count(m => m.StockBajo);

                AlertasPendientes.Clear();
                foreach (var alerta in alertas)
                    AlertasPendientes.Add(alerta);
                SinAlertas = AlertasPendientes.Count == 0;
            }
            catch (Exception ex)
            {
                _errorHandler.HandleError(ex);
            }
            finally
            {
                IsRefreshing = false;
            }
        }

        [RelayCommand]
        private Task MarcarAtendida(SeguimientoAlerta alerta)
            => CambiarEstado(alerta, EstadoAlerta.Atendida);

        [RelayCommand]
        private Task Descartar(SeguimientoAlerta alerta)
            => CambiarEstado(alerta, EstadoAlerta.Descartada);

        private async Task CambiarEstado(SeguimientoAlerta alerta, EstadoAlerta estado)
        {
            try
            {
                await _api.ActualizarEstadoAlertaAsync(alerta.Id, estado);
                AlertasPendientes.Remove(alerta);
                SinAlertas = AlertasPendientes.Count == 0;
            }
            catch (Exception ex)
            {
                _errorHandler.HandleError(ex);
            }
        }

        [RelayCommand]
        private Task VerPaciente(SeguimientoAlerta alerta)
            => Shell.Current.GoToAsync($"paciente?id={alerta.ColaboradorId}");

        [RelayCommand]
        private async Task CerrarSesion()
        {
            _sesion.CerrarSesion();
            await Shell.Current.GoToAsync("//login");
        }
    }
}
