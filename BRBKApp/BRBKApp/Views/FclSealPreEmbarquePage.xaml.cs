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
    public partial class FclSealPreEmbarquePage : ContentPage
    {
        FclSealPreEmbarqueViewModel _viewModel;
        public FclSealPreEmbarquePage()
        {
            InitializeComponent();
            this.BindingContext = _viewModel = new FclSealPreEmbarqueViewModel();
        }
    }
}