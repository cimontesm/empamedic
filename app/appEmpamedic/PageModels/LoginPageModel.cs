using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace appEmpamedic.PageModels
{
    public partial class LoginPageModel : ObservableObject
    {
        private readonly SesionService _sesion;

        [ObservableProperty]
        private string _usuario = string.Empty;

        [ObservableProperty]
        private string _clave = string.Empty;

        [ObservableProperty]
        private string? _mensajeError;

        [ObservableProperty]
        private bool _isBusy;

        public LoginPageModel(SesionService sesion)
        {
            _sesion = sesion;
        }

        [RelayCommand]
        private async Task IniciarSesion()
        {
            if (IsBusy)
                return;

            try
            {
                IsBusy = true;
                MensajeError = null;

                if (!await _sesion.IniciarSesionAsync(Usuario, Clave))
                {
                    MensajeError = "Ingrese usuario y contraseña.";
                    return;
                }

                Clave = string.Empty;
                await Shell.Current.GoToAsync("//dashboard");
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
