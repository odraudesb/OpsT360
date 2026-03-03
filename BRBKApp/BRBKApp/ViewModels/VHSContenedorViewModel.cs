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

namespace BRBKApp.ViewModels
{
	public class VHSContenedorViewModel : BaseViewModel
    {
        public ObservableCollection<VHSDataModel<Contenedor>> Contenedores { get; set; }


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
        public bool IsRefreshing
        {
            get { return isRefreshing; }
            set
            {
                isRefreshing = value;
                OnPropertyChanged();
            }
        }
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
        public VHSContenedorViewModel ()
		{
            this.Title = "Contenedor VHS";
            _btnIcon = ImageSource.FromFile("icon_add.png");
            OnPropertyChanged(nameof(BtnIcon));
            //CommandTarja = new Command<int>(ToTarja);
            //CommandContenedor = new Command<int>(ToContenedor);
            //AddCommand = new Command<string>(OnAddTap);
            LoadContenedores().ConfigureAwait(true);
            IsRefreshing = false;
            esActivo = true;
            esVisible = true;
        }
        async Task RefreshItemsAsync()
        {
            if (esActivo)
            {
                IsRefreshing = true;
                await Task.Delay(TimeSpan.FromSeconds(RefreshDuration));
                //await LoadOrdenes();
                IsRefreshing = false;
            }
        }
        private async Task LoadContenedores()
        {
            esActivo = false;
            IsRefreshing = true;
            ObservableCollection<VHSDataModel<Contenedor>> resps = new ObservableCollection<VHSDataModel<Contenedor>>();
            resps = await ServiceVHS.GetContenedores();
            Contenedores = resps;
            IsRefreshing = false;
            esActivo = true;
        }
    }
}
