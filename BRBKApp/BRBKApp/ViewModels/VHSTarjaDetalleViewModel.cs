using Acr.UserDialogs;
using ApiModels.AppModels;
using BRBKApp.DA;
using BRBKApp.Models;
using BRBKApp.Views;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace BRBKApp.ViewModels
{
    public class VHSTarjaDetalleViewModel : BaseViewModel
    {
        public ObservableCollection<VHSTarjaDetalleMensaje> Detalle { get; set; }
        public bool IsListaVacia
        {
            get
            {
                return Detalle == null || Detalle.Count == 0;
            }
        }
        public VHSTarjaMensaje Model { get; set; }
        public int IdTarjaCurrentModel => (Model==null) ? -1 : Model.TarjaId;
        public string MensajeCurrentModel => (Model == null) ? "" : Model.Mensaje;

        bool isRefreshing;
        const int RefreshDuration = 2;
        public bool _esActivo;
        public bool esActivo
        {
            get => _esActivo;
            set
            {
                _esActivo = value;
                OnPropertyChanged();
            }
        }
        private ImageSource _btnIcon;
        public ImageSource BtnIcon
        {
            get { return _btnIcon; }
            set
            {
                _btnIcon = value;
                SetProperty(ref _btnIcon, value);
            }
        }
        public ICommand AddCommand => new Command(async () => await AddDetail());
        public ICommand RefreshCommand => new Command(async () => await RefreshItemsAsync());
        public VHSTarjaDetalleViewModel(VHSTarjaMensaje tarjamodel)
        {
            Model = tarjamodel;
            this.Title = string.Format("Detalle de Tarja: {0}",tarjamodel.TarjaId);
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
                //await GetAgenda(TxtMRN);
                await LoadData();
                IsRefreshing = false;
            }
        }
        public bool IsRefreshing
        {
            get { return isRefreshing; }
            set
            {
                isRefreshing = value;
                OnPropertyChanged();
            }
        }

        private async Task LoadData()
        {
            esActivo = false;
            IsRefreshing = true;
            var resps = await ServiceVHS.GetTarjaDetail(Model.OrdenTrabajoID);
            //var resps = await ServiceVHS.GetTarjaDetail(1005);
            Detalle = new ObservableCollection<VHSTarjaDetalleMensaje>(resps.Respuesta.Detalle);
            IsRefreshing = false;
            esActivo = true;
            OnPropertyChanged(nameof(Detalle));
            OnPropertyChanged(nameof(IsListaVacia));
        }

        private async Task AddDetail()
        {
            if (Model != null)
            {
                Services.NavigationService.AddDetailTarja = new VHSTarjaMensaje()
                {
                    TarjaId = Model.TarjaId,
                    OrdenTrabajoID = Model.OrdenTrabajoID 
                };
                await Shell.Current.Navigation.PushModalAsync(new VHSTarjaAddDetail(Model));
            }
        }


        public ICommand ConsultaCommand => new Command<int>(async (detalleTarjaId) => await ConsultarDetalle(detalleTarjaId));


        private async Task ConsultarDetalle(int detalleTarjaId)
        {
            if (esActivo && Model != null)
            {
                // Crear un modelo para pasar a la página de consulta
                var tarjaMensaje = new VHSTarjaMensaje
                {
                    TarjaId = Model.TarjaId,
                    OrdenTrabajoID = Model.OrdenTrabajoID,
                    DetalleTarjaID = detalleTarjaId
                };
                // Navegar a la página de consulta
                await Shell.Current.Navigation.PushModalAsync(new VHSTarjaConsultaDetail(tarjaMensaje));
            }
        }
    }
}
