using ApiModels.AppModels;
using BRBKApp.DA;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;

namespace BRBKApp.ViewModels
{
    public class ShellViewModel : BaseViewModel
    {
        private string menuHeaderName = null;
        public string MenuHeaderName
        {
            get => menuHeaderName;
            set => SetProperty(ref menuHeaderName, value);
        }
        public ShellViewModel()
        {
            CargaMenu();
            AbrirTarjaNovedades = new Command(async () =>
            {

                await Shell.Current.GoToAsync("VHSTarjaConsultaDetail?origen=Novedad");
            });
            AbrirCediNovedades = new Command(async () =>
            {
                await Shell.Current.GoToAsync("CediTarjaConsultaDetail?origen=Novedad");
            });
        }

        
        //#################
        //      MENU
        //#################
        public bool _esActivoMn1 = false;
        public bool _esActivoMn2 = false;
        public bool _esActivoMn3 = false;
        public bool _esActivoMn4 = false;
        public bool _esActivoMn5 = false;
        public bool _esActivoMn6 = false;
        public bool esActivoMn1 // Menu BRBK
        {
            get => _esActivoMn1;
            set { _esActivoMn1 = value; OnPropertyChanged(); }
        }
        public bool esActivoMn2 // Menu MTY
        {
            get => _esActivoMn2;
            set { _esActivoMn2 = value; OnPropertyChanged(); }
        }
        public bool esActivoMn3 // Menu VBS
        {
            get => _esActivoMn3;
            set { _esActivoMn3 = value; OnPropertyChanged(); }
        }
        public bool esActivoMn4 // Menu VHS
        {
            get => _esActivoMn4;
            set { _esActivoMn4 = value; OnPropertyChanged(); }
        }


        public bool esActivoMn5 // Menu CEDI
        {
            get => _esActivoMn5;
            set { _esActivoMn5 = value; OnPropertyChanged(); }
        }

        //########################
        //     OPCIONES CEDI
        //########################
        public bool _esActivoCEDI1 = false;
        public bool esActivoCEDI1
        {
            get => _esActivoCEDI1;
            set { _esActivoCEDI1 = value; OnPropertyChanged(); }
        }
        public bool _esActivoCEDI2 = false;
        public bool esActivoCEDI2
        {
            get => _esActivoCEDI2;
            set { _esActivoCEDI2 = value; OnPropertyChanged(); }
        }
        public bool _esActivoCEDI3 = false;
        public bool esActivoCEDI3
        {
            get => _esActivoCEDI3;
            set { _esActivoCEDI3 = value; OnPropertyChanged(); }
        }
        public bool _esActivoCEDI4 = false;
        public bool esActivoCEDI4
        {
            get => _esActivoCEDI4;
            set { _esActivoCEDI4 = value; OnPropertyChanged(); }
        }
        public bool _esActivoCEDI5 = false;
        public bool esActivoCEDI5
        {
            get => _esActivoCEDI5;
            set { _esActivoCEDI5 = value; OnPropertyChanged(); }
        }
        //########################
        //     OPCIONES BRBK
        //########################
        public bool _esActivoBR1 = false;
        public bool _esActivoBR3 = false;

        public bool esActivoBR1 //Bandeja de entrada BL
        {
            get => _esActivoBR1;
            set { _esActivoBR1 = value; OnPropertyChanged(); }
        }

        public bool esActivoBR3 //Load Truck
        {
            get => _esActivoBR3;
            set { _esActivoBR3 = value; OnPropertyChanged(); }
        }

        //########################
        //     OPCIONES SEAL
        //########################
        public bool _esActivoMT1 = false;
        public bool _esActivoMT2 = false;
        public bool _esActivoMT3 = false;
        public bool _esActivoMT4 = false;
        public bool _esActivoMT5 = false;
        public bool _esActivoMT6 = false;
        public bool _esActivoMT7 = false;
        public bool _esActivoMT8 = false;

        public bool esActivoMT1 //Validación Sello(IMPO)
        {
            get => _esActivoMT1;
            set { _esActivoMT1 = value; OnPropertyChanged(); }
        }

        public bool esActivoMT2 //Asignación Sello (EXPO)
        {
            get => _esActivoMT2;
            set { _esActivoMT2 = value; OnPropertyChanged(); }
        }
        public bool esActivoMT3 //POW
        {
            get => _esActivoMT3;
            set { _esActivoMT3 = value; OnPropertyChanged(); }
        }
        public bool esActivoMT4 //Confirmacion Despacho (EXPO)
        {
            get => _esActivoMT4;
            set { _esActivoMT4 = value; OnPropertyChanged(); }
        }

        public bool esActivoMT5 //SELLO PRE-EMBARQUE PAN (EXPO)
        {
            get => _esActivoMT5;
            set { _esActivoMT5 = value; OnPropertyChanged(); }
        }
        public bool esActivoMT6 //SELLO AFORO 
        {
            get => _esActivoMT6;
            set { _esActivoMT6 = value; OnPropertyChanged(); }
        }
        public bool esActivoMT7 //ASIGNACION SELLO  EXPO
        {
            get => _esActivoMT7;
            set { _esActivoMT7 = value; OnPropertyChanged(); }
        }
        public bool esActivoMT8 //VALIDA SELLO PATIO
        {
            get => _esActivoMT8;
            set { _esActivoMT8 = value; OnPropertyChanged(); }
        }

        //########################
        //     OPCIONES VBS
        //########################
        public bool _esActivoVB1 = false;
        public bool _esActivoVB2 = false;
        public bool _esActivoVB3 = false;
        public bool _esActivoVB4 = false;
        public bool _esActivoVB5 = false;
        public bool _esActivoVB6 = false;
        public bool _esActivoVB7 = false;
        public bool _esActivoVB8 = false;
        public bool esActivoVB1 //INBOX AISV + MOVIMIENTOS (ING - EGR)
        {
            get => _esActivoVB1;
            set { _esActivoVB1 = value; OnPropertyChanged(); }
        }
        public bool esActivoVB2 //ASIGNACION DE UBICACIÓN
        {
            get => _esActivoVB2;
            set { _esActivoVB2 = value; OnPropertyChanged(); }
        }
        public bool esActivoVB3 //CONSULTA DE CARGA
        {
            get => _esActivoVB3;
            set { _esActivoVB3 = value; OnPropertyChanged(); }
        }
        public bool esActivoVB4 //ORDEN DE DESPACHO
        {
            get => _esActivoVB4;
            set { _esActivoVB4 = value; OnPropertyChanged(); }
        }
        public bool esActivoVB5 //PRE DESPACHO
        {
            get => _esActivoVB5;
            set { _esActivoVB5 = value; OnPropertyChanged(); }
        }

        public bool esActivoVB6 //DESPACHO
        {
            get => _esActivoVB6;
            set { _esActivoVB6 = value; OnPropertyChanged(); }
        }

        public bool esActivoVB7 //EMBARQUE
        {
            get => _esActivoVB7;
            set { _esActivoVB7 = value; OnPropertyChanged(); }
        }

        public bool esActivoVB8 //ADD AISV EXTERNAL
        {
            get => _esActivoVB8;
            set { _esActivoVB8 = value; OnPropertyChanged(); }
        }

        //########################
        //     OPCIONES VHS
        //########################
        public bool _esActivoVHS1 = false;
        public bool esActivoVHS1
        {
            get => _esActivoVHS1;
            set { _esActivoVHS1 = value; OnPropertyChanged(); }
        }
        public bool _esActivoVHS2 = false;
        public bool esActivoVHS2
        {
            get => _esActivoVHS2;
            set { _esActivoVHS2 = value; OnPropertyChanged(); }
        }
        public bool _esActivoVHS3 = false;
        public bool esActivoVHS3
        {
            get => _esActivoVHS3;
            set { _esActivoVHS3 = value; OnPropertyChanged(); }
        }
        public bool _esActivoVHS4 = false;
        public bool esActivoVHS4
        {
            get => _esActivoVHS4;
            set { _esActivoVHS4 = value; OnPropertyChanged(); }
        }
        
        public bool _esActivoVHS5 = false;
        public bool esActivoVHS5
        {
            get => _esActivoVHS5;
            set { _esActivoVHS5 = value; OnPropertyChanged(); }
        }


        public bool _esActivoVHS6 = false;
        public bool esActivoVHS6
        {
            get => _esActivoVHS6;
            set { _esActivoVHS6 = value; OnPropertyChanged(); }
        }

        public ICommand AbrirTarjaNovedades { get; }
        public ICommand AbrirCediNovedades { get; }

        public async void CargaMenu()
        {
            //menuHeaderName = "Joffre Alexis Bustos";
            //await Shell.Current.GoToAsync("//LoginPage");
            try
            {
                if (App.Current.Properties.ContainsKey("UserId")==true)
                {
                    var ids = App.Current.Properties["UserId"];
                    int id= Convert.ToInt32(ids);
                    List<UserDB> users = new List<UserDB>();
                    users = await App.Database.GetRegistradoById(id);
                    if (users.Count > 0)
                    {
                        var _sUser = App.Current.Properties["SuperUser"];
                        bool superUser = bool.Parse(_sUser.ToString());

                        ////////////////////
                        //    SEGURIDAD
                        ////////////////////
                        var _idRol = App.Current.Properties["RoleId"];

                        DatosApi datos = new DatosApi();
                        List<opcionesRoles> resp = await datos.GetPermisosPorRol(long.Parse(_idRol.ToString()));

                        bool esActivoBR2 = false;
                        foreach(opcionesRoles item  in resp)
                        {
                            
                            if (item.RoleId > 0)
                            {
                                menuHeaderName = item.Role_Name;
                                if (item.Selection.Trim() == "BR1") { esActivoBR1 = true; esActivoMn1 = true; }
                                if (item.Selection.Trim() == "BR2") { esActivoBR2 = true; esActivoMn1 = true; }
                                if (item.Selection.Trim() == "BR3") { esActivoBR3 = true; esActivoMn1 = true; }
                                if (item.Selection.Trim() == "MT1") { esActivoMT1 = true; esActivoMn2 = true; }
                                if (item.Selection.Trim() == "MT2") { esActivoMT2 = true; esActivoMn2 = true; }
                                if (item.Selection.Trim() == "MT3") { esActivoMT3 = true; esActivoMn2 = true; }
                                if (item.Selection.Trim() == "MT4") { esActivoMT4 = true; esActivoMn2 = true; }
                                if (item.Selection.Trim() == "MT5") { esActivoMT5 = true; esActivoMn2 = true; }
                                if (item.Selection.Trim() == "MT6") { esActivoMT6 = true; esActivoMn2 = true; }
                                if (item.Selection.Trim() == "MT7") { esActivoMT7 = true; esActivoMn2 = true; }
                                if (item.Selection.Trim() == "MT8") { esActivoMT8 = true; esActivoMn2 = true; }
                                if (item.Selection.Trim() == "VB1") { esActivoVB1 = true; esActivoMn3 = true; }
                                if (item.Selection.Trim() == "VB2") { esActivoVB2 = true; esActivoMn3 = true; }
                                if (item.Selection.Trim() == "VB3") { esActivoVB3 = true; esActivoMn3 = true; }
                                if (item.Selection.Trim() == "VB4") { esActivoVB4 = true; esActivoMn3 = true; }
                                if (item.Selection.Trim() == "VB5") { esActivoVB5 = true; esActivoMn3 = true; }
                                if (item.Selection.Trim() == "VB6") { esActivoVB6 = true; esActivoMn3 = true; }
                                if (item.Selection.Trim() == "VB7") { esActivoVB7 = true; esActivoMn3 = true; }
                                if (item.Selection.Trim() == "VB8") { esActivoVB8 = true; esActivoMn3 = true; }
                                if (item.Selection.Trim() == "VHS1") { esActivoVHS1 = true; esActivoMn4 = true; }
                                if (item.Selection.Trim() == "VHS2") { esActivoVHS2 = true; esActivoMn4 = true; }
                                if (item.Selection.Trim() == "VHS3") { esActivoVHS3 = true; esActivoMn4 = true; }
                                if (item.Selection.Trim() == "VHS4") { esActivoVHS4 = true; esActivoMn4 = true; }
                                if (item.Selection.Trim() == "VHS5") { esActivoVHS5 = true; esActivoMn4 = true; }
                                if (item.Selection.Trim() == "VHS6") { esActivoVHS6 = true; esActivoMn4 = true; }
                                if (item.Selection.Trim() == "CEDI1") { esActivoCEDI1 = true; esActivoMn5 = true; }
                                if (item.Selection.Trim() == "CEDI2") { esActivoCEDI2 = true; esActivoMn5 = true; }
                                if (item.Selection.Trim() == "CEDI3") { esActivoCEDI3 = true; esActivoMn5 = true; }
                                if (item.Selection.Trim() == "CEDI4") { esActivoCEDI4 = true; esActivoMn5 = true; }
                                if (item.Selection.Trim() == "CEDI5") { esActivoCEDI5 = true; esActivoMn5 = true; }
                                
                            }
                        }

                        App.Current.Properties["BR2"] = esActivoBR2;

                        //if (!superUser)
                        //{
                        //    menuHeaderName = "OPC - ";
                        //}
                        //else
                        //{
                        //    menuHeaderName = "CGSA - ";
                        //}

                        foreach (var item in users)
                        {
                            menuHeaderName = menuHeaderName + item.Names;
                        }

                        //_esActivo = superUser;

                        OnPropertyChanged(nameof(MenuHeaderName));
                        OnPropertyChanged(nameof(esActivoMn1));
                        OnPropertyChanged(nameof(esActivoMn2));
                        OnPropertyChanged(nameof(esActivoMT1));
                        OnPropertyChanged(nameof(esActivoMT2));
                        OnPropertyChanged(nameof(esActivoMT3));
                        OnPropertyChanged(nameof(esActivoMT4));
                        OnPropertyChanged(nameof(esActivoMT5));
                        OnPropertyChanged(nameof(esActivoBR1));
                        OnPropertyChanged(nameof(esActivoBR3));
                        OnPropertyChanged(nameof(esActivoVB1));
                        OnPropertyChanged(nameof(esActivoVB2));
                        OnPropertyChanged(nameof(esActivoVB3));
                        OnPropertyChanged(nameof(esActivoVB4));
                        OnPropertyChanged(nameof(esActivoVB5));
                        OnPropertyChanged(nameof(esActivoVB6));
                        OnPropertyChanged(nameof(esActivoVB7));
                        OnPropertyChanged(nameof(esActivoVB8));
                        OnPropertyChanged(nameof(esActivoVHS1));
                        OnPropertyChanged(nameof(esActivoVHS2));
                        OnPropertyChanged(nameof(esActivoVHS3));
                        OnPropertyChanged(nameof(esActivoVHS4));
                        OnPropertyChanged(nameof(esActivoVHS5));
                        OnPropertyChanged(nameof(esActivoVHS6));
                        OnPropertyChanged(nameof(esActivoCEDI1));
                        OnPropertyChanged(nameof(esActivoCEDI2));
                        OnPropertyChanged(nameof(esActivoCEDI3));
                        OnPropertyChanged(nameof(esActivoCEDI4));
                        OnPropertyChanged(nameof(esActivoCEDI5));
                    }
                }
                else
                {
                    await Xamarin.Essentials.SecureStorage.SetAsync("isLogged", "0");
                    App.Current.Properties["IsLoggedIn"] = false;
                    App.Current.Properties["UserId"] = null;
                }
            }
            catch (Exception)
            {
                await Shell.Current.GoToAsync("//LoginPage");
            }


        }
    }
}
