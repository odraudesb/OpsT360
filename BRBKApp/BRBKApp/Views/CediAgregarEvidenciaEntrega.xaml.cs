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
    public partial class CediAgregarEvidenciaEntrega : ContentPage
    {
        public CediAgregarEvidenciaEntrega(long vehiculoDespachadoId)
        {
            InitializeComponent();
            BindingContext = new CediAgregarEvidenciaViewModel(vehiculoDespachadoId);
        }
    }
}