using ApiModels.AppModels;
using BRBKApp.DA;
using BRBKApp.Views;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace BRBKApp.ViewModels
{
    public class CediTarjaDetalleViewModel : BaseViewModel
    {
        public ObservableCollection<CediTarjaDetalleMensaje> Detalle { get; set; }
        public bool IsListaVacia => Detalle == null || Detalle.Count == 0;

        public CediTarjaMensaje Model { get; set; }
        public int IdTarjaCurrentModel => Model == null ? -1 : Model.TarjaId;
        public string MensajeCurrentModel => Model == null ? string.Empty : Model.Mensaje;

        bool isRefreshing;
        const int RefreshDuration = 2;
        bool _esActivo;
        public bool esActivo
        {
            get => _esActivo;
            set { _esActivo = value; OnPropertyChanged(); }
        }

        ImageSource _btnIcon;
        public ImageSource BtnIcon
        {
            get => _btnIcon;
            set { _btnIcon = value; SetProperty(ref _btnIcon, value); }
        }

        public ICommand AddCommand => new Command(async () => await AddDetail());

        public ICommand RefreshCommand => new Command(async () => await RefreshItemsAsync());

        public CediTarjaDetalleViewModel(CediTarjaMensaje tarja)
        {
            Model = tarja;
            Title = $"Detalle de Tarja: {tarja.TarjaId}";
            _btnIcon = ImageSource.FromFile("icon_add.png");
            OnPropertyChanged(nameof(BtnIcon));
            LoadData().ConfigureAwait(true);
            IsRefreshing = false;
            esActivo = true;
        }

        async Task RefreshItemsAsync()
        {
            if (esActivo)
            {
                IsRefreshing = true;
                await Task.Delay(TimeSpan.FromSeconds(RefreshDuration));
                await LoadData();
                IsRefreshing = false;
            }
        }

        public bool IsRefreshing
        {
            get => isRefreshing;
            set { isRefreshing = value; OnPropertyChanged(); }
        }

        async Task LoadData()
        {
            esActivo = false;
            IsRefreshing = true;
            var resps = await ServiceCedi.GetTarjaDetail(Model.OrdenTrabajoID);
            Detalle = new ObservableCollection<CediTarjaDetalleMensaje>(resps.Respuesta.Detalle);
            IsRefreshing = false;
            esActivo = true;
            OnPropertyChanged(nameof(Detalle));
            OnPropertyChanged(nameof(IsListaVacia));
        }

        private async Task AddDetail()
        {
            if (Model != null)
            {
                await Shell.Current.Navigation.PushModalAsync(new CediTarjaAddDetail(Model));
            }
        }

        public ICommand ConsultaCommand => new Command<int>(async (detalleTarjaId) => await ConsultarDetalle(detalleTarjaId));

        private async Task ConsultarDetalle(int detalleTarjaId)
        {
            if (esActivo && Model != null)
            {
                var tarjaMensaje = new CediTarjaMensaje
                {
                    TarjaId = Model.TarjaId,
                    OrdenTrabajoID = Model.OrdenTrabajoID,
                    DetalleTarjaID = detalleTarjaId
                };
                await Shell.Current.Navigation.PushModalAsync(new CediTarjaConsultaDetail(tarjaMensaje));
            }
        }
    }
}
