using ApiModels.AppModels;
using BRBKApp.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace BRBKApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class VBSShipmentDetailsPage : ContentPage
    {
        VBSShipmentDetailsViewModel _viewModel;
        public VBSShipmentDetailsPage()
        {
            InitializeComponent();
            this.BindingContext = _viewModel = new VBSShipmentDetailsViewModel();

        }
        protected override void OnAppearing()
        {
            base.OnAppearing();
            _viewModel.IsRefreshing = true;
        }
    }
}
