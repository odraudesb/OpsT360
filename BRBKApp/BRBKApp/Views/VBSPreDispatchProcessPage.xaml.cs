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
    public partial class VBSPreDispatchProcessPage : ContentPage
    {
        VBSPreDispatchProcessViewModel _viewModel;
        public VBSPreDispatchProcessPage()
        {
            InitializeComponent();
            this.BindingContext = _viewModel = new VBSPreDispatchProcessViewModel();

        }
        protected override void OnAppearing()
        {
            base.OnAppearing();
            _viewModel.IsRefreshing = true;
        }

        private void Entry_TextChanged(object sender, TextChangedEventArgs e)
        {
            _viewModel.OnAddTap(null);
        }
    }
}