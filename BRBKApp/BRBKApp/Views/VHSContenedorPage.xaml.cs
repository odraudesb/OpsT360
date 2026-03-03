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
	public partial class VHSContenedorPage : ContentPage
	{
        VHSContenedorViewModel _viewModel;
        public VHSContenedorPage ()
		{
			InitializeComponent ();
            this.BindingContext = _viewModel = new VHSContenedorViewModel();
        }
        protected override void OnAppearing()
        {
            base.OnAppearing();
            _viewModel.IsRefreshing = true;
        }
    }
}