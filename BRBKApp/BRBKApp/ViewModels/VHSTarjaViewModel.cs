using Acr.UserDialogs;
using ApiModels.AppModels;
using BRBKApp.DA;
using BRBKApp.Views;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;
using BRBKApp.Models;
using ApiDatos;
using System.Collections.Generic;
using ViewModel;
using System.Linq;
using Android.Content.Res;

namespace BRBKApp.ViewModels
{
    public class VHSTarjaViewModel : BaseViewModel
    {
        public ObservableCollection<VHSTarjaMensaje> TarjaList { get; set; }
        public bool IsListaVacia
        {
            get
            {
                return TarjaList == null || TarjaList.Count == 0;
            }
        }
        bool isRefreshing;
        const int RefreshDuration = 2;
        public bool _esActivo;
        public bool _esVisible;
        private ImageSource _btnIcon;
        public Command ConsultCommand { get; }
        public Command NavTaskCommand { get; set; }
        public Command CommandTarja { get; }
        public Command CommandContenedor { get; }
        public Command AddCommand { get; }
        public ObservableCollection<Tasks> MyAgenda { get; private set; }
        public ImageSource BtnIcon
        {
            get { return _btnIcon; }
            set
            {
                _btnIcon = value;
                SetProperty(ref _btnIcon, value);
            }
        }
        public bool esActivo
        {
            get => _esActivo;
            set
            {
                _esActivo = value;
                OnPropertyChanged();
            }
        }
        public bool esVisible
        {
            get => _esVisible;
            set
            {
                _esVisible = value;
                OnPropertyChanged();
            }
        }

        public ICommand RefreshCommand => new Command(async () => await RefreshItemsAsync());

        public VHSTarjaViewModel()
        {
            Title = "Tarja VHS";
            _btnIcon = ImageSource.FromFile("icon_add.png");
            OnPropertyChanged(nameof(BtnIcon));
            CommandTarja = new Command<VHSTarjaMensaje>(ClickTarja);
            LoadTarja().ConfigureAwait(true);
            IsRefreshing = false;
            esActivo = true;
            esVisible = true;
        }

        async private void ClickTarja(VHSTarjaMensaje entry)
        {
            if (entry != null)
            {
                Services.NavigationService.TarjaModelToDetailPage = entry;
                await Shell.Current.GoToAsync($"VHSTarjaDetallePage");
                //await App.Current.MainPage.DisplayAlert("Info", string.Format("Evento Tarja [{0}]", entry.Mensaje), "OK");
                //await Application.Current.MainPage.Navigation.PushAsync(new VHSTarjaCrear());

            }
            /*
            try
            {
                await Shell.Current.GoToAsync("VHSTarjaDetallePage");
            }
            catch (Exception ex)
            {
                await App.Current.MainPage.DisplayAlert("Error", ex.Message, "Cerrar");
            }
            */
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
            get { return isRefreshing; }
            set
            {
                isRefreshing = value;
                OnPropertyChanged();
            }
        }

        public async Task LoadTarja()
        {
            esActivo = false;
            IsRefreshing = true;
            TarjaList = await ServiceVHS.GetTarjaList();
            IsRefreshing = false;
            esActivo = true;
            OnPropertyChanged(nameof(TarjaList));
            OnPropertyChanged(nameof(IsListaVacia));
        }

    }
}
