using System;
using System.IO;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace BRBKApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class OpsSealOperationsPage : ContentPage
    {
        public OpsSealOperationsPage()
        {
            InitializeComponent();
            Device.BeginInvokeOnMainThread(() => ScannerEntry.Focus());
        }

        private void ScannerEntry_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(e.NewTextValue))
            {
                return;
            }

            LastScanLabel.Text = $"Último código leído: {e.NewTextValue}";
        }

        private void ReadSeal1_Clicked(object sender, EventArgs e)
        {
            Seal1Entry.Text = ScannerEntry.Text;
        }

        private async void TakePhoto_Clicked(object sender, EventArgs e)
        {
            try
            {
                var photo = await MediaPicker.CapturePhotoAsync();
                if (photo == null)
                {
                    PhotoStatusLabel.Text = "No se tomó foto.";
                    return;
                }

                using (var stream = await photo.OpenReadAsync())
                {
                    var memory = new MemoryStream();
                    await stream.CopyToAsync(memory);
                    memory.Position = 0;
                    SealImagePreview.Source = ImageSource.FromStream(() => new MemoryStream(memory.ToArray()));
                }

                PhotoStatusLabel.Text = $"image: {photo.FileName}";
            }
            catch (FeatureNotSupportedException)
            {
                PhotoStatusLabel.Text = "La cámara no está soportada en este dispositivo.";
            }
            catch (PermissionException)
            {
                PhotoStatusLabel.Text = "Permisos de cámara no concedidos.";
            }
            catch (Exception ex)
            {
                PhotoStatusLabel.Text = $"Error al tomar foto: {ex.Message}";
            }
        }
    }
}
