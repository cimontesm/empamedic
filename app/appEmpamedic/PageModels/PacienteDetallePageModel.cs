using CommunityToolkit.Mvvm.ComponentModel;

namespace appEmpamedic.PageModels
{
    public partial class PacienteDetallePageModel : ObservableObject, IQueryAttributable
    {
        private readonly IEmpamedicApi _api;
        private readonly IErrorHandler _errorHandler;

        [ObservableProperty]
        private Colaborador? _paciente;

        [ObservableProperty]
        private List<ConsultaMedica> _consultas = [];

        public PacienteDetallePageModel(IEmpamedicApi api, IErrorHandler errorHandler)
        {
            _api = api;
            _errorHandler = errorHandler;
        }

        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            if (query.TryGetValue("id", out var valor) && int.TryParse(valor?.ToString(), out var id))
                Cargar(id).FireAndForgetSafeAsync(_errorHandler);
        }

        private async Task Cargar(int id)
        {
            Paciente = await _api.ObtenerColaboradorAsync(id);
            if (Paciente is null)
            {
                _errorHandler.HandleError(new InvalidOperationException("No se encontró el paciente."));
                await Shell.Current.GoToAsync("..");
                return;
            }

            Consultas = await _api.ObtenerConsultasAsync(id);
        }
    }
}
