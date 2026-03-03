using BRBKApp.DA;
using ApiModels.AppModels;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using BRBKApp.Views;

namespace BRBKApp.ViewModels
{
    public class CediOrdenTrabajoViewModel : BaseViewModel
    {
        public ObservableCollection<CediOrdenTrabajo> OrdenesTrabajo { get; set; }
        public bool IsListaVacia => OrdenesTrabajo == null || OrdenesTrabajo.Count == 0;
        bool isRefreshing;
        const int RefreshDuration = 2;
        public bool esActivo;
        public bool esVisible;

        public Command ButtonCommand { get; }

        public ICommand RefreshCommand => new Command(async () => await RefreshItemsAsync());

        public CediOrdenTrabajoViewModel()
        {
            Title = "Orden de trabajo CEDI";
            LoadOrdenes().ConfigureAwait(true);
            isRefreshing = false;
            esActivo = true;
            esVisible = true;
            ButtonCommand = new Command<CediOrdenTrabajo>(OnBotonPresionado);
        }

        async Task RefreshItemsAsync()
        {
            if (esActivo)
            {
                IsRefreshing = true;
                await Task.Delay(TimeSpan.FromSeconds(RefreshDuration));
                await LoadOrdenes();
                IsRefreshing = false;
            }
        }

        public bool IsRefreshing
        {
            get => isRefreshing;
            set { isRefreshing = value; OnPropertyChanged(); }
        }

        public async Task LoadOrdenes()
        {
            esActivo = false;
            IsRefreshing = true;

            var resps = await ServiceCedi.GetOrdenesPendientes();
            OrdenesTrabajo = new ObservableCollection<CediOrdenTrabajo>(resps.ToList());
            OrdenesTrabajo.CollectionChanged += (s, e) => OnPropertyChanged(nameof(IsListaVacia));
            OnPropertyChanged(nameof(OrdenesTrabajo));
            OnPropertyChanged(nameof(IsListaVacia));

            IsRefreshing = false;
            esActivo = true;
        }

        public async void OnBotonPresionado(CediOrdenTrabajo entry)
        {
            if (entry != null)
            {
                await Shell.Current.Navigation.PushModalAsync(new Views.CediTarjaCrear(entry));
            }
        }
    }
}
