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
    public partial class VHSTarjaAddDetail : ContentPage
    {
        VHSTarjaAddDetailViewModel _viewModel;
        public VHSTarjaAddDetail(VHSTarjaMensaje entry)
        {
            InitializeComponent();
            this.BindingContext = _viewModel = new VHSTarjaAddDetailViewModel(entry);
        }

    }
}