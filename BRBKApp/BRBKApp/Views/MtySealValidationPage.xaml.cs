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
    public partial class MtySealValidationPage : ContentPage
    {
        MtySealValidationViewModel _viewModel;
        public MtySealValidationPage()
        {
            InitializeComponent();
            this.BindingContext = _viewModel = new MtySealValidationViewModel();
        }
        protected override void OnAppearing()
        {
            base.OnAppearing();
            _viewModel.IsRefreshing = true;
        }
    }
}