namespace appEmpamedic.Pages
{
    public partial class PacientesPage : ContentPage
    {
        private readonly PacientesPageModel _model;

        public PacientesPage(PacientesPageModel model)
        {
            InitializeComponent();
            BindingContext = _model = model;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await _model.CargarAsync();
        }
    }
}
