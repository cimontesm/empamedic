using appEmpamedic.Models;
using appEmpamedic.PageModels;

namespace appEmpamedic.Pages
{
    public partial class MainPage : ContentPage
    {
        public MainPage(MainPageModel model)
        {
            InitializeComponent();
            BindingContext = model;
        }
    }
}