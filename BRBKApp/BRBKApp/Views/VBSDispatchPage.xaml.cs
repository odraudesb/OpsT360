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
    public partial class VBSDispatchPage : ContentPage
    {
        VBSDispatchViewModel _viewModel;
        public VBSDispatchPage()
        {
            InitializeComponent();
            this.BindingContext = _viewModel = new VBSDispatchViewModel();
        }

        private void Entry_TextChanged(object sender, TextChangedEventArgs e)
        {
            _viewModel.OnConsultClicked(null);
        }
    }
}