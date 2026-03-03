using Acr.UserDialogs;
using ApiModels.AppModels;
using BRBKApp.DA;
using BRBKApp.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace BRBKApp.ViewModels
{
    public class TaskDetailsViewModel : BaseViewModel
    {
        private ImageSource _btnIcon;
        public bool _esVisible;
        public string idWork;
        public string comentary;
        private string taskslabel;
        private string container;
        private string temperature;
        private string types;
        private tarjaDet _selectedItem;
        public string iduser;
        private string itemId;
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
        public string Comentary
        {
            get => comentary;
            set => SetProperty(ref comentary, value);
        }
        public string TasksLabel{
            get => taskslabel;
            set => SetProperty(ref taskslabel, value);
        }
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
        
        public List<TasksBD> Listregistrado { get; private set; }

        public Command AddCommand { get; }
        
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

        public async void LoadItemId(tarjaDet oDet, int arrastre)
        {
            try
            {
                //GetNovedades();
                await CargaDetalle(oDet, arrastre);
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

        public TaskDetailsViewModel()
        {
            Title = "Discharge Detail";
            _btnIcon = ImageSource.FromFile("icon_add.png");
            OnPropertyChanged(nameof(BtnIcon));
            var ids = App.Current.Properties["UserId"];
            iduser = ids.ToString();
            NavTaskCommand = new Command<string>(NavTaskDetail);
            
            AddCommand = new Command<string>(OnAddTap);
            ItemId = SecureStorage.GetAsync("WorkId").Result;
            GetAgenda(ItemId);

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
            await GetAgenda(ItemId);
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

        private async Task CargaDetalle(tarjaDet oDet, int arrastre)
        {
            if (oDet != null)
            {
                _selectedItem = oDet;
                int vSaldo = (int)oDet.cantidad; //- arrastre;
                vSaldo = vSaldo - arrastre;
                container = "BL: " + oDet.carga;
                taskslabel = "Consignee: " + oDet.Consignatario;
                temperature = "Commodity: "+ oDet.producto?.nombre;
                types = "Qty: " + oDet.cantidad + "     Drag: " + arrastre.ToString() + "    Pending Qty: " + vSaldo.ToString();
                comentary = "Planned Location: " + oDet.Ubicaciones?.nombre;
                idWork = oDet.idTarjaDet.ToString();
            }
            else
            {
                container = "BL: MRN-MSN-HSN";
                temperature = "Qty = 0";
                types = "Drag: 0";
                taskslabel = "Consignee: XXXXXXXXXXX XXXXXXXXX XXXXXXXXXXX";
                comentary = "";
                idWork = "0";
            }
            OnPropertyChanged(nameof(Container));
            OnPropertyChanged(nameof(Temperature));
            OnPropertyChanged(nameof(Types));
            OnPropertyChanged(nameof(TasksLabel));
            OnPropertyChanged(nameof(Comentary));

        }
        private async Task GetAgenda(string idSeleccioando)
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
                    tarjaDet oDet;
                    int _arrastre;

                    var _sUser = App.Current.Properties["SuperUser"];
                    bool superUser = bool.Parse(_sUser.ToString());
                    string _lugar = string.Empty;

                    if (superUser)
                    {
                        _lugar = "BODEGA";
                    }
                    else
                    {
                        _lugar = "MUELLE";
                    }

                    MyAgenda = await datos.GetListaRecepciones(long.Parse(idSeleccioando), id,_lugar);
                    oDet = datos.oDet;
                    _arrastre = datos.arrastre;

                    OnPropertyChanged(nameof(MyAgenda));

                    LoadItemId(oDet, _arrastre);
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
        async private void NavTaskDetail(string obj)
        {
            var _sUser = App.Current.Properties["SuperUser"];
            bool superUser = bool.Parse(_sUser.ToString());

            if (!superUser)
            {
                await SecureStorage.SetAsync("WorkId", obj);
                await Shell.Current.GoToAsync("/TaskDetails/AlertReportPage");
            }
            else
            {
                await SecureStorage.SetAsync("WorkId", obj);
                await Shell.Current.GoToAsync("UpdateDetail");
            }

            
        }

        async private void OnAddTap(string obj)
        {
            try
            {
                await SecureStorage.SetAsync("WorkId", idWork);
                await Shell.Current.GoToAsync("EditDetail");
            }
            catch (Exception ex)
            {
                await App.Current.MainPage.DisplayAlert("Error", ex.Message, "Cerrar");
            }
        }
    }
}
