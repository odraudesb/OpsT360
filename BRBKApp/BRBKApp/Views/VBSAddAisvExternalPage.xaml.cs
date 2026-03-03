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
    public partial class VBSAddAisvExternalPage : ContentPage
    {
        VBSAddAisvExternalViewModel _viewModel;
        public VBSAddAisvExternalPage()
        {
            InitializeComponent();
            this.BindingContext = _viewModel = new VBSAddAisvExternalViewModel();
        }
        protected override void OnAppearing()
        {
            base.OnAppearing();
            _viewModel.IsRefreshing = true;
        }

        private void Picker_SelectedIndexChanged(object sender, EventArgs e)
        {
            _viewModel.CargaListaBloque();
        }
    }
}