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
    public partial class VHSAgregarEvidenciaEntrega : ContentPage
    {
        public VHSAgregarEvidenciaEntrega(long vehiculoDespachadoId)
        {
            InitializeComponent();
            BindingContext = new VHSAgregarEvidenciaEntregaViewModel(vehiculoDespachadoId);
        }
    }
}