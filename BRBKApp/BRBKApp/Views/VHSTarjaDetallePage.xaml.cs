using ApiModels.AppModels;
using BRBKApp.Models;
using BRBKApp.Services;
using BRBKApp.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace BRBKApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class VHSTarjaDetallePage : ContentPage
    {
        VHSTarjaDetalleViewModel _viewModel;
        public VHSTarjaDetallePage()
        {
            InitializeComponent();
            var tarjaModel = NavigationService.TarjaModelToDetailPage;
            if (tarjaModel == null)
            {
                // s no se encuentra el modelo hacia el detalle
                return;
            }
            this.BindingContext = _viewModel = new VHSTarjaDetalleViewModel(tarjaModel);
        }
        protected override void OnAppearing()
        {
            base.OnAppearing();
            _viewModel.IsRefreshing = true;
        }
        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            NavigationService.TarjaModelToDetailPage = null;
        }

    }
}