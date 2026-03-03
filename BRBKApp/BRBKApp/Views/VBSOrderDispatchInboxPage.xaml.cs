using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using BRBKApp.ViewModels;
using Plugin.Geolocator;
using ApiModels.AppModels;

namespace BRBKApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class VBSOrderDispatchInboxPage : ContentPage
    {
        VBSOrderDispatchInboxViewModel _viewModel;
        public VBSOrderDispatchInboxPage()
        {
            InitializeComponent();
            this.BindingContext = _viewModel = new VBSOrderDispatchInboxViewModel();

        }
        protected override void OnAppearing()
        {
            base.OnAppearing();
            _viewModel.IsRefreshing = true;
        }
    }
}