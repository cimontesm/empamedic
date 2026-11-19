using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace appEmpamedic.PageModels
{
    public partial class PacientesPageModel : ObservableObject
    {
        private readonly IEmpamedicApi _api;
        private readonly IErrorHandler _errorHandler;
        private List<Colaborador> _todos = [];

        [ObservableProperty]
        private string _busqueda = string.Empty;

        [ObservableProperty]
        private List<Colaborador> _pacientes = [];

        [ObservableProperty]
        private bool _isRefreshing;

        public PacientesPageModel(IEmpamedicApi api, IErrorHandler errorHandler)
        {
            _api = api;
            _errorHandler = errorHandler;
        }

        [RelayCommand]
        public async Task CargarAsync()
        {
            try
            {
                IsRefreshing = true;
                _todos = await _api.ObtenerColaboradoresAsync();
                Filtrar();
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

        partial void OnBusquedaChanged(string value) => Filtrar();

        private void Filtrar()
        {
            var texto = Busqueda.Trim();
            Pacientes = string.IsNullOrEmpty(texto)
                ? _todos
                : _todos.Where(c =>
                        c.NombreCompleto.Contains(texto, StringComparison.CurrentCultureIgnoreCase) ||
                        c.Cedula.Contains(texto, StringComparison.OrdinalIgnoreCase))
                    .ToList();
        }

        [RelayCommand]
        private Task VerPaciente(Colaborador colaborador)
            => Shell.Current.GoToAsync($"paciente?id={colaborador.Id}");
    }
}
