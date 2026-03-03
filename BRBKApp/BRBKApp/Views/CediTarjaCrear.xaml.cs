using ApiModels.AppModels;
using BRBKApp.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.CommunityToolkit.Extensions;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace BRBKApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CediTarjaCrear : ContentPage
    {
        CediTarjaCrearViewModel _viewModel;
        public CediTarjaCrear(CediOrdenTrabajo ordenTrabajo)
        {
            InitializeComponent();
            _viewModel = new CediTarjaCrearViewModel(ordenTrabajo);
            this.BindingContext = _viewModel;
        }
        
    }
}