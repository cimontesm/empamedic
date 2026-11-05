namespace appEmpamedic.Pages
{
    public partial class PacienteDetallePage : ContentPage
    {
        public PacienteDetallePage(PacienteDetallePageModel model)
        {
            InitializeComponent();
            BindingContext = model;
        }
    }
}
