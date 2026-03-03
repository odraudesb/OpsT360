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
    public partial class FclSealAssignsPage : ContentPage
    {
        FclAssignsSealExpoViewModel _viewModel;
        public FclSealAssignsPage()
        {
            InitializeComponent();
            this.BindingContext = _viewModel = new FclAssignsSealExpoViewModel();
        }
    }
}