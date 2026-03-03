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
    public partial class VBSShipmentInboxNewPage : ContentPage
    {
        VBSShipmentInboxNewViewModel _viewModel;
        public VBSShipmentInboxNewPage()
        {
            InitializeComponent();
            this.BindingContext = _viewModel = new VBSShipmentInboxNewViewModel();

        }
    }
}