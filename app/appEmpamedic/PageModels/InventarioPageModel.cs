using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace appEmpamedic.PageModels
{
    public partial class InventarioPageModel : ObservableObject
    {
        private readonly IEmpamedicApi _api;
        private readonly IErrorHandler _errorHandler;

        [ObservableProperty]
        private List<Medicamento> _medicamentos = [];

        [ObservableProperty]
        private bool _isRefreshing;

        public InventarioPageModel(IEmpamedicApi api, IErrorHandler errorHandler)
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
                Medicamentos = await _api.ObtenerMedicamentosAsync();
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
    }
}
