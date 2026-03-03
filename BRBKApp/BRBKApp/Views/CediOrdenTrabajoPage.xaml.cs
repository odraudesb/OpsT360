using ApiModels.AppModels;
using BRBKApp.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace BRBKApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CediOrdenTrabajoPage : ContentPage
    {
        CediOrdenTrabajoViewModel _viewModel;
        public CediOrdenTrabajoPage()
        {
            InitializeComponent();
            _viewModel = new CediOrdenTrabajoViewModel();
            BindingContext = _viewModel;
        }
        protected override void OnAppearing()
        {
            base.OnAppearing();
            _viewModel.IsRefreshing = true;
        }
    }
}
