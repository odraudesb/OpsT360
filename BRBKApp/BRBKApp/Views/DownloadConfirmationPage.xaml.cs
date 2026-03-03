using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using BRBKApp.ViewModels;

namespace BRBKApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DownloadConfirmationPage : ContentPage
    {
        DownloadConfirmationViewModel _viewModel;
        public DownloadConfirmationPage()
        {
            InitializeComponent();
            this.BindingContext = _viewModel = new DownloadConfirmationViewModel();
        }
        protected override void OnAppearing()
        {
            base.OnAppearing();
            //_viewModel.IsRefreshing = true;
            _viewModel.esActivo = false;
            _viewModel.esActivo1 = true;
        }
    }
}