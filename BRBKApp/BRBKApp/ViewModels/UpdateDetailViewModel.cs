using ApiModels.AppModels;
using Plugin.Media;
using Plugin.Media.Abstractions;
using BRBKApp.DA;
using BRBKApp.Views;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Acr.UserDialogs;

namespace BRBKApp.ViewModels
{
    public class UpdateDetailViewModel : BaseViewModel
    {
        #region Declaraciones
        public string idWork;
        private string itemId;
        private tarjaDet _selectedItem;

        public bool _esActivo;
        public string _lugar;
        private string _qty;
        private string _observacion;
        private string container;
        private string temperature;
        private string types;
        private string taskslabel;
        public string iduser;
        public string comentary;
        private Combo _selectedNov;


        public Command ReportAlertCommand { get; }
        public Command SaveChangesCommand { get; }
        public Command CancelCommand { get; }
        public Command TapCommand { get; }
        
        public recepcion MyAgenda { get; private set; }

        public bool esActivo
        {
            get => _esActivo;
            set
            {
                _esActivo = value;
                OnPropertyChanged();
            }
        }
        public string QtyEntry
        {
            get => _qty;
            set
            {
                _qty = value;
                OnPropertyChanged();
            }
        }
        
        public string ObservacionEntry
        {
            get => _observacion;
            set
            {
                _observacion = value;
                OnPropertyChanged();
            }
        }
        public Combo SelectedNov
        {
            get { return _selectedNov; }
            set { _selectedNov = value; OnPropertyChanged(); }
        }

        public List<Combo> ListUbicaciones { get; private set; }
      

        public string Container
        {
            get => container;
            set => SetProperty(ref container, value);
        }
        
        public string Temperature
        {
            get => temperature;
            set => SetProperty(ref temperature, value);
        }
        
        public string Types
        {
            get => types;
            set => SetProperty(ref types, value);
        }
        
        public string TasksLabel
        {
            get => taskslabel;
            set => SetProperty(ref taskslabel, value);
        }
        public string ItemId
        {
            get
            {
                return itemId;
            }
            set
            {
                itemId = value;

            }
        }
        #endregion

        public UpdateDetailViewModel()
        {
            Title = "Location Confirmation";
            var ids = App.Current.Properties["UserId"];
            iduser = ids.ToString();
            ReportAlertCommand = new Command(OnReportAlertClicked);
            SaveChangesCommand = new Command(async (o) => await OnSaveClicked(o));
            CancelCommand = new Command(OnCancelClicked);
            ItemId = SecureStorage.GetAsync("WorkId").Result;
            
            GetRecepcion(ItemId);

            //aplica permisos
            var _sUser = App.Current.Properties["SuperUser"];
            bool superUser = bool.Parse(_sUser.ToString());

            if (!superUser)
            {
                esActivo = false;
            }
            else
            {
                esActivo = true;
            }

        }
        public async void LoadItemId(recepcion oRecepcion)
        {
            try
            {
                await CargaDetalle(oRecepcion);
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        private async void GetRecepcion(string idSeleccioando)
        {
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
                    var ids = App.Current.Properties["Username"];
                    string userName = ids.ToString();
                    MyAgenda = await datos.GetRecepcion(long.Parse(idSeleccioando), userName);
                    OnPropertyChanged(nameof(MyAgenda));
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

       
        private async Task CargaDetalle(recepcion oRec)
        {
            try
            {
                if (oRec != null)
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
                        ///////////////////////////////////
                        //   llenar combo de ubicaciones
                        ///////////////////////////////////
                        DatosApi datos = new DatosApi();
                        var dd = await datos.GetListaUbicaciones();
                        ListUbicaciones = dd;
                        SelectedNov = ListUbicaciones?.FirstOrDefault();
                        OnPropertyChanged(nameof(ListUbicaciones));

                        ///////////////////////////////////
                        //   carga datos en pantalla
                        ///////////////////////////////////
                        _selectedItem = oRec.TarjaDet;
                        container = "BL: " + _selectedItem.carga;
                        temperature = "Qty: " + _selectedItem.cantidad;
                        types = "Commodity: " + _selectedItem.producto?.nombre;//"Ubicación: " + _selectedItem.Ubicaciones?.nombre;
                        taskslabel = "Consignee: " + _selectedItem.Consignatario;
                        comentary = "";
                        idWork = oRec.idRecepcion.ToString();

                        QtyEntry = oRec.cantidad.ToString();
                        SelectedNov = ListUbicaciones?.Where(a => a.Valor == oRec.ubicacion).FirstOrDefault();
                        ObservacionEntry = "";// oRec.observacion;
                    }
                }
                else
                {
                    container = "BL: MRN-MSN-HSN";
                    temperature = "Cantidad = 0";
                    types = "Arrastre: 0";
                    taskslabel = "Cliente: XXXXXXXXXXX XXXXXXXXX XXXXXXXXXXX";
                    comentary = "";
                    idWork = "0";
                }

                OnPropertyChanged(nameof(Container));
                OnPropertyChanged(nameof(Temperature));
                OnPropertyChanged(nameof(Types));
                OnPropertyChanged(nameof(TasksLabel));
                OnPropertyChanged(nameof(ListUbicaciones));
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
        private async Task OnSaveClicked(object obj)
        {
            try
            {
                var ids = App.Current.Properties["Username"];
                string userName = ids.ToString();

                bool DeACuerdo;
                bool flags = false;
                DeACuerdo = await App.Current.MainPage.DisplayAlert("Confirmar", "¿Desea registrar?", "De Acuerdo", "Cancelar");
                IsBusy = true;
                if (DeACuerdo)
                {
                    if (QtyEntry == null || QtyEntry == "" || QtyEntry == "0")
                    {
                        await App.Current.MainPage.DisplayAlert("Error", "Please enter QTY", "Close");
                        flags = true;
                    }

                    if (SelectedNov== null)
                    {
                        await App.Current.MainPage.DisplayAlert("Error", "Please select Ubicación", "Close");
                        flags = true;
                    }

                    if (string.IsNullOrEmpty(ObservacionEntry))
                    {
                        await App.Current.MainPage.DisplayAlert("Error", "Please enter Note", "Close");
                        flags = true;
                    }

                    if (flags == false)
                    {
                        UserDialogs.Instance.ShowLoading("Processing...");
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

                            double tmp = Convert.ToDouble(QtyEntry);
                            QtyEntry = tmp.ToString("0.##");

                            recepcion oRecepcion = new recepcion();
                            oRecepcion.idRecepcion = long.Parse(idWork);
                            oRecepcion.idTarjaDet = long.Parse(_selectedItem.idTarjaDet.ToString());
                            oRecepcion.cantidad = decimal.Parse(QtyEntry.ToString());
                            string idUbicacion = SelectedNov?.Valor;
                            oRecepcion.ubicacion = idUbicacion;
                            oRecepcion.observacion = ObservacionEntry;
                            oRecepcion.estado = "NUE";
                            oRecepcion.usuarioModifica = userName;

                            ApiModels.AppModels.Base msg = await datos.ActualizarRecepcion(oRecepcion).ConfigureAwait(true);
                            await App.Current.MainPage.DisplayAlert("Respuesta", msg.messages, "Close");
                            if (msg.response == true)
                            {
                                IsBusy = false;
                                await Shell.Current.GoToAsync("////InboxPage/TaskDetails");
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
        private async void OnReportAlertClicked(object sender)
        {
            if (_selectedItem == null)
                return;

            await Shell.Current.GoToAsync("/TaskDetails/AlertReportPage");
            //await Shell.Current.Navigation.PushAsync(new AlertReportPage());

        }
        private async void OnCancelClicked()
        {
            await Shell.Current.GoToAsync("////InboxPage/TaskDetails");
        }
    }
}
