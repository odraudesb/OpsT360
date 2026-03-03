using ApiModels.AppModels;
using BRBKApp.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using static Android.Webkit.WebStorage;

namespace BRBKApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    [Xamarin.Forms.QueryProperty(nameof(Origen), "origen")]
    public partial class VHSTarjaConsultaDetail : TabbedPage, IQueryAttributable
    {
        public string Origen { get; set; }
        private readonly VHSTarjaMensaje _entry;

        public VHSTarjaConsultaDetail(VHSTarjaMensaje entry = null)
        {
            InitializeComponent();
            _entry = entry;
            this.BindingContext = new VHSTarjaConsultaDetailViewModel(entry);
        }

        public VHSTarjaConsultaDetail()
        {
            InitializeComponent();
            _entry = null;
            this.BindingContext = new VHSTarjaConsultaDetailViewModel();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            var viewModel = this.BindingContext as VHSTarjaConsultaDetailViewModel;
            if (viewModel != null)
            {
                if (_entry != null && _entry.DetalleTarjaID > 0)
                {
                    viewModel.NumeroTarja = _entry.TarjaId.ToString();
                    viewModel.ShowCodeInput = false;
                    viewModel.ShowDetails = true;
                }
                else
                {
                    viewModel.ShowCodeInput = true;
                    viewModel.ShowDetails = false;
                }

                System.Diagnostics.Debug.WriteLine(
                    string.Format("ShowCodeInput en OnAppearing: {0}, ShowDetails: {1}",
                                  viewModel.ShowCodeInput, viewModel.ShowDetails));
            }
        }

        private async void OnImageTapped(object sender, EventArgs e)
        {
            // Verificar que el sender sea un Image
            var image = sender as Image;
            if (image == null)
            {
                Console.WriteLine("Sender is not an Image");
                await DisplayAlert("Error", "No se pudo identificar la imagen.", "OK");
                return;
            }

            Console.WriteLine("Sender is Image. GestureRecognizers count: " + image.GestureRecognizers.Count);

            // Obtener el TapGestureRecognizer
            var gesture = image.GestureRecognizers.OfType<TapGestureRecognizer>().FirstOrDefault();
            if (gesture == null)
            {
                Console.WriteLine("No TapGestureRecognizer found");
                await DisplayAlert("Error", "No se pudo identificar el gesto.", "OK");
                return;
            }

            // Obtener el CommandParameter
            string parameter = gesture.CommandParameter != null ? gesture.CommandParameter.ToString() : null;
            int index;
            if (string.IsNullOrEmpty(parameter) || !int.TryParse(parameter, out index))
            {
                Console.WriteLine("Invalid parameter: " + parameter);
                await DisplayAlert("Error", "Parámetro inválido.", "OK");
                return;
            }

            // Obtener el ViewModel
            var viewModel = this.BindingContext as VHSTarjaConsultaDetailViewModel;
            if (viewModel == null)
            {
                Console.WriteLine("ViewModel is null");
                await DisplayAlert("Error", "No se pudo acceder al ViewModel.", "OK");
                return;
            }

            // Obtener el ImageSource correspondiente según el índice
            ImageSource imageSource = null;
            switch (index)
            {
                case 0:
                    imageSource = viewModel.ImageSource0;
                    break;
                case 1:
                    imageSource = viewModel.ImageSource1;
                    break;
                case 2:
                    imageSource = viewModel.ImageSource2;
                    break;
                case 3:
                    imageSource = viewModel.ImageSource3;
                    break;
                case 4:
                    imageSource = viewModel.ImageSource4;
                    break;
                case 5:
                    imageSource = viewModel.ImageSource5;
                    break;
                case 6:
                    imageSource = viewModel.ImageSource6;
                    break;
                case 7:
                    imageSource = viewModel.ImageSource7;
                    break;
            }

            if (imageSource != null)
            {
                await Navigation.PushModalAsync(new ImageZoomPage(imageSource));
            }
            else
            {
                Console.WriteLine("ImageSource for index " + index + " is null");
                await DisplayAlert("Error", "No se pudo cargar la imagen.", "OK");
            }
        }

        private async void OnPrintClicked(object sender, EventArgs e)
        {
            var viewModel = this.BindingContext as VHSTarjaConsultaDetailViewModel;
            if (viewModel != null)
            {
                await viewModel.PrintTarja();
            }
        }

        private void Entry_Focused(object sender, FocusEventArgs e)
        {
            // El foco está en el Entry, el escáner Zebra llenará el campo automáticamente
            var entry = sender as Entry;
            if (entry != null && string.IsNullOrWhiteSpace(entry.Text))
            {
                // Opcional: feedback visual/vibración
            }
        }

        public void ApplyQueryAttributes(IDictionary<string, string> query)
        {
            if (query.ContainsKey("origen"))
            {
                string origen = query["origen"];

                var vm = this.BindingContext as VHSTarjaConsultaDetailViewModel;
                if (vm != null)
                {
                    vm.Origen = origen;
                }
            }
        }
    }
}
