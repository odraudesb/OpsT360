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
    public class VHSOrdenTrabajoViewModel : BaseViewModel
    {
        public ObservableCollection<VHSOrdenTrabajo> OrdenesTrabajo { get; set; }
        public bool IsListaVacia 
        { 
            get 
            {  
                return OrdenesTrabajo == null || OrdenesTrabajo.Count == 0;
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

        public VHSOrdenTrabajoViewModel()
        {
            Title = "Orden de trabajo VHS";
            _btnIcon = ImageSource.FromFile("icon_add.png");
            OnPropertyChanged(nameof(BtnIcon));
            CommandTarja = new Command<int>(ToTarja);
            CommandContenedor = new Command<int>(ToContenedor);
            AddCommand = new Command<string>(OnAddTap);
            LoadOrdenes().ConfigureAwait(true);
            IsRefreshing = false;
            esActivo = true;
            esVisible = true;
            ButtonCommand = new Command<VHSOrdenTrabajo>(OnBotonPresionado);
        }

        async private void ToTarja(int obj)
        {
            try
            {
                await Shell.Current.GoToAsync("VHSTarjaDetallePage");
            }
            catch (Exception ex)
            {
                await App.Current.MainPage.DisplayAlert("Error", ex.Message, "Cerrar");
            }
        }
        async private void ToContenedor(int obj)
        {
            try
            {
                await Shell.Current.GoToAsync("VHSContenedorPage");
            }
            catch (Exception ex)
            {
                await App.Current.MainPage.DisplayAlert("Error", ex.Message, "Cerrar");
            }
        }
        async public void OnAddTap(string obj)
        {
            try
            {
                await Shell.Current.GoToAsync("VHSTarjaDetallePage");
            }
            catch (Exception ex)
            {
                await App.Current.MainPage.DisplayAlert("Error", ex.Message, "Cerrar");
            }
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
            get { return isRefreshing; }
            set
            {
                isRefreshing = value;
                OnPropertyChanged();
            }
        }

        public async Task LoadOrdenes()
        {
            esActivo = false;
            IsRefreshing = true;

            var resps = await ServiceVHS.GetOrdenesPendientes();

            OrdenesTrabajo = new ObservableCollection<VHSOrdenTrabajo>(resps.ToList());

            // Escuchar cambios si modificas la colección luego
            OrdenesTrabajo.CollectionChanged += (s, e) => OnPropertyChanged(nameof(IsListaVacia));

            OnPropertyChanged(nameof(OrdenesTrabajo));
            OnPropertyChanged(nameof(IsListaVacia)); // <- Esta línea es clave

            IsRefreshing = false;
            esActivo = true;
        }

        public Command ButtonCommand { get; }
        public async void OnBotonPresionado(VHSOrdenTrabajo entry)
        {
            if (entry!=null) {
                //await App.Current.MainPage.DisplayAlert("Info", string.Format("Evento Tarja [{0}]", entry.Mensaje), "OK"); 
                //await Application.Current.MainPage.Navigation.PushAsync(new VHSTarjaCrear());
                //await Shell.Current.GoToAsync($"VHSTarjaCrear?numeroOrden={Uri.EscapeDataString(entry.NumeroOrden)}");
                await Shell.Current.Navigation.PushModalAsync(new VHSTarjaCrear(entry));
            }
        }
    }
}
