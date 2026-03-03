using BRBKApp.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace BRBKApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CediTarjaPage : ContentPage
    {
        CediTarjaViewModel _viewModel;
        public CediTarjaPage()
        {
            InitializeComponent();
            _viewModel = new CediTarjaViewModel();
            BindingContext = _viewModel;
        }
        protected override void OnAppearing()
        {
            base.OnAppearing();
            _viewModel.IsRefreshing = true;
        }
    }
}
