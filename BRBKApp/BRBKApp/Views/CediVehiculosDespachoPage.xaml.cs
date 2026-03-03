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
    public partial class CediVehiculosDespachoPage : ContentPage
    {
        private CediVehiculosDespachoViewModel _viewModel;

        public CediVehiculosDespachoPage()
        {
            InitializeComponent();

            // Aquí inicializas el ViewModel
            _viewModel = new CediVehiculosDespachoViewModel();
            this.BindingContext = _viewModel;


        }
        private void ListView_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (e.Item == null)
                return;

            // Obtiene la ViewModel
            var viewModel = BindingContext as CediVehiculosDespachoViewModel;
            if (viewModel != null && viewModel.ConsultarDetalleCommand != null && viewModel.ConsultarDetalleCommand.CanExecute(e.Item))
            {
                viewModel.ConsultarDetalleCommand.Execute(e.Item);
            }

         // Desmarca el item
         ((ListView)sender).SelectedItem = null;
        }
    }
}