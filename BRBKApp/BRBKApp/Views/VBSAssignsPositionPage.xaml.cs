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
    public partial class VBSAssignsPositionPage : ContentPage
    {
        VBSAssignsPositionViewModel _viewModel;
        public VBSAssignsPositionPage()
        {
            InitializeComponent();
            this.BindingContext = _viewModel = new VBSAssignsPositionViewModel();
        }

        private void txtNotas_TextChanged(object sender, TextChangedEventArgs e)
        {
            _viewModel.CargaLista();
        }

        private void txtBarcodeAdicional_TextChanged(object sender, TextChangedEventArgs e)
        {
            _viewModel.OnAdConsultClicked(null);
        }

        private void txtBarcodePallet_TextChanged(object sender, TextChangedEventArgs e)
        {
            _viewModel.OnConsultClicked(null);
        }
    }
}