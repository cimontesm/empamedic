namespace appEmpamedic.Pages
{
    public partial class DashboardPage : ContentPage
    {
        private readonly DashboardPageModel _model;

        public DashboardPage(DashboardPageModel model)
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
