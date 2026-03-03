using ApiModels.AppModels;
using BRBKApp.DA;
using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;
using BRBKApp.Views;
using System.Linq;
using System.IO;
using Plugin.Media;
using Plugin.Media.Abstractions;
using Xamarin.Essentials;
using Acr.UserDialogs;
using System.Threading.Tasks;
using System.Windows.Input;

namespace BRBKApp.ViewModels
{
    public class VBSAddAisvExternalViewModel : BaseViewModel
    {
        #region Declaraciones
        bool isRefreshing;
        public bool _esActivo;
        const int RefreshDuration = 2;
        public string idWork;
        public string _barcodeEntry;
        private ImageSource _btnIcon;
        public string iduser;

        private Combo _selectedNave;
        private Combo _selectedHold;
        private Combo _selectedBodega;
        private Combo _selectedBloque;

        public ImageSource BtnIcon
        {
            get { return _btnIcon; }
            set
            {
                _btnIcon = value;
                SetProperty(ref _btnIcon, value);
            }
        }
        
        public Command SaveChangesCommand { get; }
        public Command CancelCommand { get; }
        public Command TapCommand { get; }
        public List<TasksBD> Listregistrado { get; private set; }

        public string BarcodeEntry
        {
            get => _barcodeEntry;
            set
            {
                _barcodeEntry = value;
                OnPropertyChanged();
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

      
        public bool esActivo
        {
            get => _esActivo;
            set
            {
                _esActivo = value;
                OnPropertyChanged();
            }
        }

        public Combo SelectedNave
        {
            get { return _selectedNave; }
            set { _selectedNave = value; OnPropertyChanged(); }
        }
        public Combo SelectedHold
        {
            get { return _selectedHold; }
            set { _selectedHold = value; OnPropertyChanged(); }
        }
        public Combo SelectedBodega
        {
            get { return _selectedBodega; }
            set { _selectedBodega = value; OnPropertyChanged(); }
        }
        public Combo SelectedBloque
        {
            get { return _selectedBloque; }
            set { _selectedBloque = value; OnPropertyChanged(); }

        }
        public List<Combo> ListNaves { get; private set; }
        public List<Combo> ListHolds { get; private set; }
        public List<Combo> ListBodegas { get; private set; }
        public List<Combo> ListBloques { get; private set; }

        public ICommand RefreshCommand => new Command(async () => await RefreshItemsAsync());
        #endregion
        public VBSAddAisvExternalViewModel()
        {
            Title = "Add AISV External";
             _btnIcon = ImageSource.FromFile("icon_search.png");

            OnPropertyChanged(nameof(BtnIcon));

            SaveChangesCommand = new Command(async (o) => await OnSaveClicked(o));
            CancelCommand = new Command(OnCancelClicked);
            esActivo = true;
        }

        async Task RefreshItemsAsync()
        {
            if (esActivo)
            {
                IsRefreshing = true;
                await Task.Delay(TimeSpan.FromSeconds(RefreshDuration));

                ///////////////////////////////////
                //   llenar combo 
                ///////////////////////////////////
                CargaListaNaves();
                CargaListaHold();
                CargaListaBodega();
                IsRefreshing = false;
            }
        }
        public void CleanPages()
        {
            IsBusy = false;
            idWork = "0";
            BarcodeEntry = "";
            OnPropertyChanged(nameof(BarcodeEntry));

            ListNaves = null;
            SelectedNave = null;
            OnPropertyChanged(nameof(ListNaves));

            ListHolds = null;
            SelectedHold = null;
            OnPropertyChanged(nameof(ListHolds));

            ListBodegas = null;
            SelectedBodega = null;
            OnPropertyChanged(nameof(ListBodegas));

            ListBloques = null;
            SelectedBloque = null;
            OnPropertyChanged(nameof(ListBloques));

            CargaListaNaves();
        }
        private void OnCancelClicked(object obj)
        {
            CleanPages();
        }

        private async Task OnSaveClicked(object obj)
        {
            bool DeACuerdo;
            bool flags = false;
            try
            {
                if (string.IsNullOrEmpty(BarcodeEntry))
                {
                    await App.Current.MainPage.DisplayAlert("Error", "Please enter AISV", "Close");
                    flags = true;
                }
                
                DeACuerdo = await App.Current.MainPage.DisplayAlert("Confirmar", string.Format( "¿Está seguro de agregar el AISV {0}?", BarcodeEntry ), "De Acuerdo", "Cancelar");
                var ids = App.Current.Properties["Username"];
                string userName = ids.ToString();

                if (DeACuerdo)
                {
                    if (string.IsNullOrEmpty(SelectedNave?.Valor))
                    {
                        await App.Current.MainPage.DisplayAlert("Error", "Please enter Vessel", "Close");
                        flags = true;
                    }
                    if (string.IsNullOrEmpty(SelectedHold?.Valor))
                    {
                        await App.Current.MainPage.DisplayAlert("Error", "Please enter Hold", "Close");
                        flags = true;
                    }
                    if (string.IsNullOrEmpty(SelectedBodega?.Valor))
                    {
                        await App.Current.MainPage.DisplayAlert("Error", "Please enter Store", "Close");
                        flags = true;
                    }
                    if (string.IsNullOrEmpty(SelectedBloque?.Valor))
                    {
                        await App.Current.MainPage.DisplayAlert("Error", "Please enter Block", "Close");
                        flags = true;
                    }

                    

                    if (flags == false)
                    {
                        var current = Connectivity.NetworkAccess;
                        if (current == NetworkAccess.None)
                        {
                            await App.Current.MainPage.DisplayAlert("Internet Error", "Please verify your internet connection", "OK");
                            return;
                        }
                        if (current == NetworkAccess.Internet)
                        {
                            UserDialogs.Instance.ShowLoading("Sending...");
                            DatosApi datos = new DatosApi();

                            ApiModels.AppModels.Base msg = await datos.RegistraAisvExterno(SelectedNave.Valor,int.Parse(SelectedHold.Valor), int.Parse(SelectedBodega.Valor), int.Parse(SelectedBloque.Valor),BarcodeEntry.ToString(),userName).ConfigureAwait(true);
                            
                            if (msg.response == true)
                            {
                                await App.Current.MainPage.DisplayAlert("Respuesta", msg.messages, "Close");
                                CleanPages();
                            }
                            else
                            {
                                await App.Current.MainPage.DisplayAlert("Error", msg.messages, "Close");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                await App.Current.MainPage.DisplayAlert("Error", ex.Message, "Cerrar");
            }
            finally
            {
                UserDialogs.Instance.HideLoading();
            }
        }
    
        

        public async void CargaListaNaves()
        {
            try
            {
                DatosApi datos = new DatosApi();
                var dd = await datos.GetListaNaveST();
                if (dd.Count() > 0)
                {
                    ///////////////////////////////////
                    //   llenar combo de nave
                    ///////////////////////////////////
                    ListNaves = dd;
                    SelectedNave = ListNaves?.FirstOrDefault();
                    OnPropertyChanged(nameof(ListNaves));
                }
                else
                {
                    ListNaves = null;
                    SelectedNave = null;
                    OnPropertyChanged(nameof(ListNaves));
                }
            }
            catch
            {
                ListNaves = null;
                OnPropertyChanged(nameof(ListNaves));
            }
        }

        public async void CargaListaHold()
        {
            try
            {
                DatosApi datos = new DatosApi();
                var dd = await datos.GetListaHoldVBS();
                if (dd.Count() > 0)
                {
                    ///////////////////////////////////
                    //   llenar combo de nave
                    ///////////////////////////////////
                    ListHolds = dd;
                    SelectedHold = ListHolds?.FirstOrDefault();
                    OnPropertyChanged(nameof(ListHolds));
                }
                else
                {
                    ListHolds = null;
                    SelectedHold= null;
                    OnPropertyChanged(nameof(ListHolds));
                }
            }
            catch
            {
                ListHolds = null;
                OnPropertyChanged(nameof(ListHolds));
            }
        }

        public async void CargaListaBodega()
        {
            try
            {
                DatosApi datos = new DatosApi();
                var dd = await datos.GetListaBodegasVBS();
                if (dd.Count() > 0)
                {
                    ///////////////////////////////////
                    //   llenar combo de nave
                    ///////////////////////////////////
                    ListBodegas = dd;
                    SelectedBodega = ListBodegas?.FirstOrDefault();
                    OnPropertyChanged(nameof(ListBodegas));
                }
                else
                {
                    ListBodegas = null;
                    SelectedBodega = null;
                    OnPropertyChanged(nameof(ListBodegas));
                }
            }
            catch
            {
                ListBodegas = null;
                OnPropertyChanged(nameof(ListBodegas));
            }
        }

        public async void CargaListaBloque()
        {
            try
            {
                DatosApi datos = new DatosApi();
                var dd = await datos.GetListaBloquesVBS(int.Parse(SelectedBodega?.Valor));
                if (dd.Count() > 0)
                {
                    ///////////////////////////////////
                    //   llenar combo de bloques
                    ///////////////////////////////////
                    ListBloques = dd;
                    SelectedBloque = ListBloques?.FirstOrDefault();
                    OnPropertyChanged(nameof(ListBloques));
                }
                else
                {
                    ListBloques = null;
                    SelectedBloque = null;
                    OnPropertyChanged(nameof(ListBloques));
                }
            }
            catch
            {
                ListBloques = null;
                OnPropertyChanged(nameof(ListBloques));
            }
        }
    }
}
