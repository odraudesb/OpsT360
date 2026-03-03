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
    public partial class DispatchPage : ContentPage
    {
        DispatchViewModel _viewModel;
        public DispatchPage()
        {
            InitializeComponent();
            this.BindingContext = _viewModel = new DispatchViewModel();
        }
    }
}