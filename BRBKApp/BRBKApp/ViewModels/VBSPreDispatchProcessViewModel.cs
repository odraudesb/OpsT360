using Acr.UserDialogs;
using ApiModels.AppModels;
using BRBKApp.DA;
using BRBKApp.Views;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace BRBKApp.ViewModels
{
    public class VBSPreDispatchProcessViewModel : BaseViewModel
    {
        private ImageSource _btnIcon;
        public bool _esVisible;
        public string idWork;
        private BAN_Consulta_FilasPorOrden _selectedItem;
        public string iduser;
        private string itemId;
        private string txtMrn = null;
        bool isRefreshing;
        const int RefreshDuration = 2;

        public ImageSource BtnIcon
        {
            get { return _btnIcon; }
            set
            {
                _btnIcon = value;
                SetProperty(ref _btnIcon, value);
            }
        }

        public string TxtMRN
        {
            get => txtMrn;
            set
            {
                txtMrn = value;
                OnPropertyChanged();
            }
        }

        public List<TasksBD> Listregistrado { get; private set; }

        public Command AddCommand { get; }
        public Command CancelCommand { get; }

        public string ItemId
        {
            get
            {
                return itemId;
            }
            set
            {
                itemId = value;
                //LoadItemId(value);
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

        public async void LoadItemId(BAN_Consulta_FilasPorOrden oEntidad, int arrastre)
        {
            try
            {
                //GetNovedades();
                await CargaDetalle(oEntidad, arrastre);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public string _temperature;
        public string TemperatureEntry
        {
            get => _temperature;
            set
            {
                _temperature = value;
                OnPropertyChanged();
            }
        }
        public string _ventilation;
        public string VentilationEntry
        {
            get => _ventilation;
            set
            {
                _ventilation = value;
                OnPropertyChanged();
            }
        }

        //CancellationTokenSource cts;
        public Command NavTaskCommand { get; set; }
        public ObservableCollection<Tasks> MyAgenda { get; private set; }
        public ICommand RefreshCommand => new Command(async () => await RefreshItemsAsync());

        public VBSPreDispatchProcessViewModel()
        {
            Title = "Pre Dispatch Process";
            _btnIcon = ImageSource.FromFile("icon_add.png");
            OnPropertyChanged(nameof(BtnIcon));
            var ids = App.Current.Properties["UserId"];
            iduser = ids.ToString();
            AddCommand = new Command<string>(OnAddTap);
            CancelCommand = new Command(OnCancelClicked);
            ItemId = SecureStorage.GetAsync("WorkId").Result;

            BAN_Consulta_FilasPorOrden oEntidad = JsonConvert.DeserializeObject<BAN_Consulta_FilasPorOrden>(ItemId.ToString());


            GetAgenda(oEntidad);

            //aplica permisos
            var _sUser = App.Current.Properties["SuperUser"];
            bool superUser = bool.Parse(_sUser.ToString());

            if (!superUser)
            {
                esVisible = true;
            }
            else
            {
                esVisible = true;
            }
        }

        async Task RefreshItemsAsync()
        {
            IsRefreshing = true;
            await Task.Delay(TimeSpan.FromSeconds(RefreshDuration));
            BAN_Consulta_FilasPorOrden oEntidad = JsonConvert.DeserializeObject<BAN_Consulta_FilasPorOrden>(ItemId.ToString());
            await GetAgenda(oEntidad);
            IsRefreshing = false;
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

        private async Task CargaDetalle(BAN_Consulta_FilasPorOrden oDet, int arrastre)
        {
            if (oDet != null)
            {
                _selectedItem = oDet;
                idWork = ItemId;
            }
            else
            {
                idWork = "0";
            }
        }
        private async Task GetAgenda(BAN_Consulta_FilasPorOrden oSeleccionado)
        {
            try
            {
                var current = Connectivity.NetworkAccess;
                if (current == NetworkAccess.None)
                {
                    await App.Current.MainPage.DisplayAlert("Internet Error", "Please verify your internet connection", "OK");
                    return;
                }
                if (current == NetworkAccess.Internet)
                {
                    UserDialogs.Instance.ShowLoading("Loading...");
                    DatosApi datos = new DatosApi();
                    var ids = App.Current.Properties["UserId"];
                    int id = Convert.ToInt32(ids);

                    int _arrastre;

                    var _sUser = App.Current.Properties["SuperUser"];
                    bool superUser = bool.Parse(_sUser.ToString());

                    MyAgenda = await datos.GetListaMovimientosPorFila(oSeleccionado.idOrdenDespacho,oSeleccionado.idFila );

                    _arrastre = datos.arrastre;

                    OnPropertyChanged(nameof(MyAgenda));

                    LoadItemId(oSeleccionado, _arrastre);
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

        async public void OnAddTap(string obj)
        {
            try
            {
                if (TxtMRN.Length == 13)
                {
                    try
                    {
                        var _sUser = App.Current.Properties["SuperUser"];
                        bool superUser = bool.Parse(_sUser.ToString());

                        var ids = App.Current.Properties["Username"];
                        string userName = ids.ToString();

                        bool DeACuerdo;
                        bool flags = false;
                        DeACuerdo = await App.Current.MainPage.DisplayAlert("Confirmar", "¿Desea Pre Despachar Carga?", "De Acuerdo", "Cancelar");
                        IsBusy = true;
                        if (DeACuerdo)
                        {
                            if (string.IsNullOrEmpty(TxtMRN))
                            {
                                await App.Current.MainPage.DisplayAlert("Error", "Check QR Pallets", "Close");
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

                                    var oMov = MyAgenda.Where(p => p.Status == TxtMRN.ToString()).FirstOrDefault();

                                    TxtMRN = string.Empty;
                                    OnPropertyChanged(nameof(TxtMRN));
                                    if (oMov is null)
                                    {
                                        await App.Current.MainPage.DisplayAlert("Error", "Please verify the barcode", "OK");
                                        return;
                                    }
                                    else
                                    {

                                        ApiModels.AppModels.Base msg = await datos.RegistraPreDespacho(long.Parse(oMov.Id), userName).ConfigureAwait(true);
                                        await App.Current.MainPage.DisplayAlert("Respuesta", msg.messages, "Close");
                                        if (msg.response == true)
                                        {
                                            await RefreshItemsAsync();
                                            
                                            //if (MyAgenda is null)
                                            //{
                                            //    await Shell.Current.GoToAsync("////VBSOrderDispatchInboxPage/VBSOrderDispatchDetailsPage");
                                            //}
                                            //else
                                            //{
                                            if (MyAgenda?.Count == 0)
                                            {
                                                await Shell.Current.GoToAsync("////VBSPreDispatchInboxPage/VBSPreDispatchDetailsPage");
                                            }
                                            IsBusy = false;
                                            //}
                                        }
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        await App.Current.MainPage.DisplayAlert("Error", ex.Message, "Close");
                    }
                    finally
                    {
                        UserDialogs.Instance.HideLoading();
                    }
                }
            }
            catch (Exception ex)
            {
                await App.Current.MainPage.DisplayAlert("Error", ex.Message, "Cerrar");
            }
        }

        private async void OnCancelClicked()
        {
            await Shell.Current.GoToAsync("////VBSPreDispatchInboxPage/VBSPreDispatchDetailsPage");
        }

    }
}
