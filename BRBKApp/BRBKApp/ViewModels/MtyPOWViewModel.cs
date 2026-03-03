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
using BRBKApp.Services;

namespace BRBKApp.ViewModels
{
    public class MtyPOWViewModel : BaseViewModel
    {
        #region Declaraciones
        private ImageSource _btnIcon;
        private Combo _selectedNave;
        private Combo _selectedGrua;
        private List<Combo> _gruas;
        public ImageSource BtnIcon
        {
            get { return _btnIcon; }
            set
            {
                _btnIcon = value;
                SetProperty(ref _btnIcon, value);
            }
        }

        public Combo SelectedNave
        {
            get { return _selectedNave; }
            set 
            {
                _selectedNave = value; OnPropertyChanged();
                
                List<Combo> oListGrua = _gruas?.Where(a => a.Descripcion == SelectedNave?.Descripcion).ToList();

                ListGrua = oListGrua;

                SelectedGrua = ListGrua?.FirstOrDefault();
                OnPropertyChanged(nameof(ListGrua));
                OnPropertyChanged(nameof(SelectedGrua));
            }
        }

        public Combo SelectedGrua
        {
            get { return _selectedGrua; }
            set { _selectedGrua = value; OnPropertyChanged(); }
        }
        public List<Combo> ListNave { get; private set; }
        public List<Combo> ListGrua { get; private set; }

        public Command SaveChangesCommand { get; }
        public Command CancelCommand { get; }
        public Command TapCommand { get; }

        #endregion
        public MtyPOWViewModel()
        {
            Title = "Point of Work";
             _btnIcon = ImageSource.FromFile("icon_search.png");
            OnPropertyChanged(nameof(BtnIcon));

            SaveChangesCommand = new Command(async (o) => await OnSaveClicked(o));
            CancelCommand = new Command(OnCancelClicked);
            CargaListas();

        }
       
        public void CleanPages()
        {
            IsBusy = false;
            SelectedNave = null;
            SelectedGrua = null;
         
            OnPropertyChanged(nameof(SelectedNave));
            OnPropertyChanged(nameof(SelectedGrua));
          
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
                DeACuerdo = await App.Current.MainPage.DisplayAlert("Confirm", "¿Are you Sure?", "Ok", "Cancel");

                var ids = App.Current.Properties["Username"];
                string userName = ids.ToString();

                if (DeACuerdo)
                {
                    if  (SelectedNave == null)
                        {
                        await App.Current.MainPage.DisplayAlert("Error", "Select vessel visit", "Close");
                        flags = true;
                        return;
                    }

                    if (_selectedGrua == null)
                    {
                        await App.Current.MainPage.DisplayAlert("Error", "Select Crane", "Close");
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

                            string imeis = "";
                            imeis = DependencyService.Get<IServiceImei>().GetImei();

                            var v_IP = App.Current.Properties["UserIp"];
                            workPosition oPOW = new workPosition();
                            oPOW.ip = v_IP.ToString();
                            oPOW.imei = imeis;
                            oPOW.idPosition = SelectedNave.Descripcion.ToString();
                            oPOW.namePosition = SelectedGrua.Valor.ToString();
                            oPOW.estado = true;
                            oPOW.usuarioCrea = userName.ToLower();

                            ApiModels.AppModels.Base msg = await datos.SetPOWRegisters(oPOW).ConfigureAwait(true);
                            await App.Current.MainPage.DisplayAlert("Result", msg.messages, "Close");
                            //if (msg.response == true)
                            //{
                                CleanPages();
                            //}
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

        private async Task CargaListas()
        {
            ///////////////////////////////////////////
            //   llenar combo de Referencia y Gruas
            ///////////////////////////////////////////
            DatosApi datos = new DatosApi();
            var dd = await datos.GetListPOWN4();
            var lstNave = (from A in dd.GroupBy(A => A.Descripcion)
                        select new 
                        { 
                            Descripcion = A.Key
                        }).Distinct();

            ListNave = new List<Combo>();

            foreach (var item in lstNave)
            {
                ListNave.Add(new Combo
                {
                    Valor = item.Descripcion,
                    Descripcion = item.Descripcion
                });
            }

            ListGrua = dd;
            _gruas = ListGrua;
            SelectedNave = ListNave?.FirstOrDefault();
            OnPropertyChanged(nameof(ListNave));
            OnPropertyChanged(nameof(ListGrua));
            OnPropertyChanged(nameof(SelectedNave));


        }
    }
}
