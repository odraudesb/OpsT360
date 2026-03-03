using ApiModels.AppModels;
using BRBKApp.Services;
using BRBKApp.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace BRBKApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CediTarjaDetallePage : ContentPage
    {
        CediTarjaDetalleViewModel _viewModel;
        public CediTarjaDetallePage()
        {
            InitializeComponent();
            var tarjaModel = NavigationService.CediTarjaModelToDetailPage;
            if (tarjaModel == null)
            {
                return;
            }
            BindingContext = _viewModel = new CediTarjaDetalleViewModel(tarjaModel);
        }
        protected override void OnAppearing()
        {
            base.OnAppearing();
            if (_viewModel != null) _viewModel.IsRefreshing = true;
        }
        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            NavigationService.CediTarjaModelToDetailPage = null;
        }
    }
}
