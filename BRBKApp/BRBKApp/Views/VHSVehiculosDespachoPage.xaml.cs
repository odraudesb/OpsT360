using ApiModels.AppModels;
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
    public partial class VHSVehiculosDespachoPage : ContentPage
    {
        private VHSVehiculosDespachoViewModel _viewModel;

        public VHSVehiculosDespachoPage()
        {
            InitializeComponent();

            // Aquí inicializas el ViewModel
            _viewModel = new VHSVehiculosDespachoViewModel();
            this.BindingContext = _viewModel;


        }
        private void ListView_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (e.Item == null)
                return;

            // Obtiene la ViewModel
            var viewModel = BindingContext as VHSVehiculosDespachoViewModel;
            if (viewModel != null && viewModel.ConsultarDetalleCommand != null && viewModel.ConsultarDetalleCommand.CanExecute(e.Item))
            {
                viewModel.ConsultarDetalleCommand.Execute(e.Item);
            }

         // Desmarca el item
         ((ListView)sender).SelectedItem = null;
        }
    }
}