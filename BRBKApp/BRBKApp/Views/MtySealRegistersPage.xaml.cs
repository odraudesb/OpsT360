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
    public partial class MtySealRegistersPage : ContentPage
    {
        MtySealRegistersViewModel _viewModel;
        public MtySealRegistersPage()
        {
            InitializeComponent();
            this.BindingContext = _viewModel = new MtySealRegistersViewModel();
        }
    }
}