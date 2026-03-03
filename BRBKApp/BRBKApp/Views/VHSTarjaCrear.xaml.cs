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
    public partial class VHSTarjaCrear : ContentPage
    {
        VHSTarjaCrearViewModel _viewModel;
        public VHSTarjaCrear(VHSOrdenTrabajo ordenTrabajo)
        {
            InitializeComponent();
            _viewModel = new VHSTarjaCrearViewModel(ordenTrabajo);
            this.BindingContext = _viewModel;
        }
        
    }
}