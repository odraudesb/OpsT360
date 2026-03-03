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
using System.Collections.ObjectModel;

namespace BRBKApp.ViewModels
{
    public class DownloadConfirmationViewModel : BaseViewModel
    {
        #region Declaraciones

        bool isRefreshing;
        const int RefreshDuration = 2;
        public bool _esActivo;
        public bool _esActivo1;
        private ImageSource _btnIcon;
        private dataContainers _selectedItem;
        private string txtNumContainer = null;
        private string txtPosition = null;
        private string _dataContainers = null;

        public long gkey{ get; set; }
        public string referencia { get; set; }

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
        public bool esActivo1
        {
            get => _esActivo1;
            set
            {
                _esActivo1 = value;
                OnPropertyChanged();
            }
        }
        public string TxtNumContainer
        {
            get => txtNumContainer;
            set
            {
                txtNumContainer = value;
                OnPropertyChanged();
            }
        }
        public string TxtPosition
        {
            get => txtPosition;
            set
            {
                txtPosition = value;
                OnPropertyChanged();
            }
        }
        public string DataContainers
        {
            get => _dataContainers;
            set => SetProperty(ref _dataContainers, value);
        }
        public Command ConsultCommand { get; }
        public Command SaveChangesCommand { get; }
        public Command CancelCommand { get; }
        public bool IsRefreshing
        {
            get { return isRefreshing; }
            set
            {
                isRefreshing = value;
                OnPropertyChanged();
            }
        }
        public async void LoadItemId(ObservableCollection<dataContainers> oDet)
        {
            try
            {
                await CargaDetalle(oDet);
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public ObservableCollection<dataContainers> MyAgenda { get; private set; }
        public List<TasksBD> Listregistrado { get; private set; }

        #endregion

        public ICommand RefreshCommand => new Command(async () => await RefreshItemsAsync());
        public DownloadConfirmationViewModel()
        {
            Title = "Download Confirmation";
             _btnIcon = ImageSource.FromFile("icon_search.png");
            OnPropertyChanged(nameof(BtnIcon));

            SaveChangesCommand = new Command(async (o) => await OnSaveClicked(o));
            CancelCommand = new Command(OnCancelClicked);
            ConsultCommand = new Command(OnConsultClicked);
            esActivo = false;
            esActivo1 = !esActivo;
            OnPropertyChanged(nameof(esActivo));
            OnPropertyChanged(nameof(esActivo1));
        }
       
        public void CleanPages()
        {
            IsBusy = false;
            TxtNumContainer = "";
            txtPosition = "";
            esActivo = false;
            esActivo1 = !esActivo;
            gkey = -1;
            referencia = string.Empty;
            _dataContainers = string.Empty;

            OnPropertyChanged(nameof(TxtNumContainer));
            OnPropertyChanged(nameof(TxtPosition));
            OnPropertyChanged(nameof(esActivo));
            OnPropertyChanged(nameof(esActivo1));
            OnPropertyChanged(nameof(DataContainers));
        }

        private async void OnConsultClicked(object obj)
        {
            esActivo = false;
            esActivo1 = !esActivo;
            OnPropertyChanged(nameof(esActivo));
            OnPropertyChanged(nameof(esActivo1));
            IsRefreshing = true;
            await Task.Delay(TimeSpan.FromSeconds(RefreshDuration));
           // await GetAgenda(TxtMRN);
            IsRefreshing = false;
            //esActivo = true; ;
        }
        async Task RefreshItemsAsync()
        {
            IsRefreshing = true;

            var ids = App.Current.Properties["Username"];
            string userName = ids.ToString();

            await Task.Delay(TimeSpan.FromSeconds(RefreshDuration));
            await GetAgenda(null,userName);
            IsRefreshing = false;
        }

        private async Task GetAgenda(string idPosition, string _user)
        {
            if (string.IsNullOrEmpty(_user)) { return; }
            try
            {
                UserDialogs.Instance.ShowLoading("Loading...");
                var current = Connectivity.NetworkAccess;
                if (current == NetworkAccess.None)
                {
                    await App.Current.MainPage.DisplayAlert("Internet Error", "Please verify your internet connection", "OK");
                    return;
                }
                if (current == NetworkAccess.Internet)
                {
                    DatosApi datos = new DatosApi();
                  

                    MyAgenda = await datos.GetDataConteinersImpo(txtNumContainer);
                    //OnPropertyChanged(nameof(MyAgenda));
                    LoadItemId(MyAgenda);
                }
            }
            catch
            {
                App.Current.Properties["IsLoggedIn"] = false;
                App.Current.Properties["UserId"] = null;
                App.Current.Properties["Timers"] = 1;
                await Xamarin.Essentials.SecureStorage.SetAsync("isLogged", "0");
                await Shell.Current.GoToAsync("//LoginPage");
            }
            finally
            {
                UserDialogs.Instance.HideLoading();
            }
        }
        private async Task CargaDetalle(ObservableCollection<dataContainers> oDet)
        {
            if (esActivo) { return; }

            if (oDet != null)
            {
                esActivo = false;
                esActivo1 = !esActivo;
                _selectedItem = oDet.FirstOrDefault();

                if (!string.IsNullOrEmpty(txtNumContainer) && _selectedItem.visit != null)
                {
                    //_dataContainers = string.Format("[PORT:{0} | WEIGHT:{1} | TYPE:{2} | °C:{3} | IMO:{4} | REF:{5}]", _selectedItem.puerto1, _selectedItem.peso, _selectedItem.tipo, _selectedItem.temperatura, _selectedItem.imo, _selectedItem.visit);

                    _dataContainers = _selectedItem.notas;

                    await App.Current.MainPage.DisplayAlert("Validation", "Successful", "OK");
                    esActivo = true;
                    esActivo1 = !esActivo;

                    gkey = _selectedItem.gkey;
                    referencia = _selectedItem.visit;
                }
                else
                {
                    esActivo = false;
                    esActivo1 = !esActivo;
                    if (!string.IsNullOrEmpty(txtNumContainer))
                    {
                        await App.Current.MainPage.DisplayAlert("Validation", "Unit not found :" + TxtNumContainer, "OK");
                    }
                }
            }
            else
            {
                _dataContainers = string.Empty;
            }
            OnPropertyChanged(nameof(DataContainers));
            OnPropertyChanged(nameof(esActivo));
            OnPropertyChanged(nameof(esActivo1));
        }
        private async void OnCancelClicked(object obj)
        {
            CleanPages();
        }
        private async Task OnSaveClicked(object obj)
        {
            try
            {
                bool DeACuerdo;
                bool flags = false;

             
                if (!string.IsNullOrEmpty(txtNumContainer) && string.IsNullOrEmpty(DataContainers))
                {
                    await App.Current.MainPage.DisplayAlert("Error", "Please search the Container", "Close");
                    flags = true;
                    return;
                }

                DeACuerdo = await App.Current.MainPage.DisplayAlert("Confirm", "¿Are you sure?", "Yes", "Cancel");

                var ids = App.Current.Properties["Username"];
                string userName = ids.ToString();

                if (DeACuerdo)
                {
                    if (string.IsNullOrEmpty(TxtNumContainer))
                    {
                        await App.Current.MainPage.DisplayAlert("Error", "Please enter Containers", "Close");
                        flags = true;
                        return;
                    }

                    if (string.IsNullOrEmpty(TxtPosition))
                    {
                        await App.Current.MainPage.DisplayAlert("Error", "Please enter Positions", "Close");
                        flags = true;
                        return;
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
                            var idUser = App.Current.Properties["UserId"];
                            int id = Convert.ToInt32(idUser);

                            downloadConfirmation odc = new downloadConfirmation();
                            odc.gkey = gkey;
                            odc.container = TxtNumContainer;
                            odc.dataContainer = DataContainers;
                            odc.position = TxtPosition;
                            odc.referencia = referencia;
                            odc.usuarioCrea = userName.ToLower();

                            ApiModels.AppModels.Base msg = await datos.SetDownloadConfirmation(odc).ConfigureAwait(true);
                            await App.Current.MainPage.DisplayAlert("Respuesta", msg.messages, "Close");
                            if (msg.response == true)
                            {
                                CleanPages();
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

       
    }
}
