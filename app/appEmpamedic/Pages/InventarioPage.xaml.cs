namespace appEmpamedic.Pages
{
    public partial class InventarioPage : ContentPage
    {
        private readonly InventarioPageModel _model;

        public InventarioPage(InventarioPageModel model)
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
