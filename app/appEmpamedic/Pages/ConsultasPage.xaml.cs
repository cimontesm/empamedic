namespace appEmpamedic.Pages
{
    public partial class ConsultasPage : ContentPage
    {
        private readonly ConsultasPageModel _model;

        public ConsultasPage(ConsultasPageModel model)
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
