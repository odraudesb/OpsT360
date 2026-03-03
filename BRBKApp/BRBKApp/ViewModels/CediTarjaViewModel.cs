using BRBKApp.DA;
using ApiModels.AppModels;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace BRBKApp.ViewModels
{
    public class CediTarjaViewModel : BaseViewModel
    {
        public ObservableCollection<CediTarjaMensaje> TarjaList { get; set; }
        public bool IsListaVacia => TarjaList == null || TarjaList.Count == 0;
        bool isRefreshing;
        const int RefreshDuration = 2;
        public bool esActivo;
        public bool esVisible;

        public ICommand RefreshCommand => new Command(async () => await RefreshItemsAsync());
        public Command<CediTarjaMensaje> CommandTarja { get; }

        public CediTarjaViewModel()
        {
            Title = "Tarja CEDI";
            CommandTarja = new Command<CediTarjaMensaje>(ClickTarja);
            LoadTarja().ConfigureAwait(true);
            isRefreshing = false;
            esActivo = true;
            esVisible = true;
        }

        async Task RefreshItemsAsync()
        {
            if (esActivo)
            {
                IsRefreshing = true;
                await Task.Delay(TimeSpan.FromSeconds(RefreshDuration));
                await LoadTarja();
                IsRefreshing = false;
            }
        }

        public bool IsRefreshing
        {
            get => isRefreshing;
            set { isRefreshing = value; OnPropertyChanged(); }
        }

        public async Task LoadTarja()
        {
            esActivo = false;
            IsRefreshing = true;
            TarjaList = await ServiceCedi.GetTarjaList();
            IsRefreshing = false;
            esActivo = true;
            OnPropertyChanged(nameof(TarjaList));
            OnPropertyChanged(nameof(IsListaVacia));
        }

        async void ClickTarja(CediTarjaMensaje entry)
        {
            if (entry != null)
            {
                Services.NavigationService.CediTarjaModelToDetailPage = entry;
                await Shell.Current.GoToAsync("CediTarjaDetallePage");
            }
        }
    }
}
