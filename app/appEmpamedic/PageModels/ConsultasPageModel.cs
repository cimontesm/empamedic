using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace appEmpamedic.PageModels
{
    public partial class ConsultasPageModel : ObservableObject
    {
        private readonly IEmpamedicApi _api;
        private readonly IErrorHandler _errorHandler;

        [ObservableProperty]
        private List<ConsultaMedica> _consultas = [];

        [ObservableProperty]
        private bool _isRefreshing;

        public ConsultasPageModel(IEmpamedicApi api, IErrorHandler errorHandler)
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
                Consultas = await _api.ObtenerConsultasAsync();
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
        private Task VerPaciente(ConsultaMedica consulta)
            => Shell.Current.GoToAsync($"paciente?id={consulta.ColaboradorId}");
    }
}
