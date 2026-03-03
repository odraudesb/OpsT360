using System;
using System.Text;
using Xamarin.Forms;
using Zebra.Sdk.Comm;
using Zebra.Sdk.Printer;
using Zebra.Sdk.Printer.Discovery;
using BRBKApp.ViewModels;
using BRBKApp.Services;
//using Windows.Devices.Bluetooth;
using System.Collections.Generic;
using System.Threading.Tasks;
using Android.Bluetooth;
//using Android.Telecom;
//using Android.Content;
using Zebra.Sdk.Graphics.Shared;

using System.IO;
using Xamarin.Forms.Xaml;
using Zebra.Sdk.Graphics;


[assembly: Dependency(typeof(PrintService))]
namespace BRBKApp.ViewModels
{
    public class PrintService //: IPrintService
    {
        string v_Device;
        string v_MacAddressEntry;
        public async void Print(string textToPrint)
        {
            BluetoothAdapter bluetoothAdapter = BluetoothAdapter.DefaultAdapter;

            if (bluetoothAdapter == null || !bluetoothAdapter.IsEnabled)
            {
                // Bluetooth no está disponible o no está habilitado
                await App.Current.MainPage.DisplayAlert("Error", "Bluetooth not found", "Close");
                return;
            }

            var pairedDevices = bluetoothAdapter.BondedDevices;
            if (pairedDevices != null && pairedDevices.Count > 0)
            {
                foreach (var device1 in pairedDevices)
                {
                    // Aquí puedes listar los dispositivos emparejados
                    //Console.WriteLine(device1.Name + " - " + device1.Address);

                    if (device1.Name.ToUpper().Contains("ZQ"))//--> NOMBRE DE BLUETOOTH ZQ230
                    {
                        v_Device = device1.Name;
                        v_MacAddressEntry = device1.Address;
                    }
                }
            }

            if (string.IsNullOrEmpty(v_MacAddressEntry))
            {
                await App.Current.MainPage.DisplayAlert("Error", "MacAddress Zebra not found", "Close");
                return;
            }

            bool DeACuerdo;
            DeACuerdo = await App.Current.MainPage.DisplayAlert("Confirmar", string.Format("¿Desea imprmir la etiqueta del pallet? \nImpresora: {0} \nMacAddress: {1}", v_Device, v_MacAddressEntry), "De Acuerdo", "Cancelar");
            if (DeACuerdo)
            {
                string bluetoothMacAddress = v_MacAddressEntry; // Reemplaza con la dirección MAC de tu impresora
                Connection printerConnection = null;

                try
                {
                    // Crear la conexión Bluetooth
                    printerConnection = DependencyService.Get<IConnectionManager>().GetBluetoothConnection(bluetoothMacAddress);

                    // Abrir la conexión
                    printerConnection.Open();
                    var printer1 = ZebraPrinterFactory.GetInstance(printerConnection);
                    var printerStatus = printer1.GetCurrentStatus();

                    // Obtener la instancia de la impresora
                    ZebraPrinter printer = ZebraPrinterFactory.GetInstance(printerConnection);

                    if (printer.GetCurrentStatus().isReadyToPrint)
                    {
                        // Crear la cadena ZPL para imprimir
                        //string zplData = "^XA^FO50,50^ADN,36,20^FD" + textToPrint + "^FS^XZ";
                        //zplData = TestLabelZpl;
                        // Enviar los datos a la impresora
                        printerConnection.Write(Encoding.UTF8.GetBytes(textToPrint));
                    }
                    else
                    {
                        //Console.WriteLine("Printer is not ready to print.");
                        await App.Current.MainPage.DisplayAlert("Error", "Printer is not ready to print.", "Close");
                    }
                }
                catch (ConnectionException e)
                {
                    printerConnection.Write(Encoding.UTF8.GetBytes(textToPrint));
                    //Console.WriteLine("Connection exception: " + e.Message);
                    await App.Current.MainPage.DisplayAlert("Error", "Connection exception: " + e.Message, "Close");
                }
                catch (ZebraPrinterLanguageUnknownException e)
                {
                    //Console.WriteLine("Zebra printer language unknown exception: " + e.Message);
                    await App.Current.MainPage.DisplayAlert("Error", "Zebra printer language unknown exception: " + e.Message, "Close");
                }
                finally
                {
                    // Cerrar la conexión
                    if (printerConnection != null && printerConnection.Connected)
                    {
                        try
                        {
                            printerConnection.Close();
                        }
                        catch (ConnectionException e)
                        {
                            //Console.WriteLine("Connection exception during closing: " + e.Message);
                            await App.Current.MainPage.DisplayAlert("Error", "Connection exception during closing: " + e.Message, "Close");
                        }
                    }
                }
            }
        }

        public string ZPLDesignEtiqueta(string _barcodePallet, string _gateInDate, string _booking, string _aisv, string _exporter, string _vessel, string _modality, string _dae, string _box)
        {
            string v_ZPL = string.Empty;

            v_ZPL = @"^XA";
            v_ZPL = v_ZPL + " " +  "^FO30,30^BQN,2,9";
            v_ZPL = v_ZPL + " " +  string.Format("^FDQA,{0};DATE:{1};BOOKING:{2};AISV:{3};EXPORTER:{4};VESSEL:{5};MODALITY:{6};DAE:{7};BOXES:{8}^FS", _barcodePallet, _gateInDate, _booking, _aisv, _exporter, _vessel, _modality, _dae, _box);
            v_ZPL = v_ZPL + " " + string.Format("^FO130,590^A0N,50,50^FD{0}^FS",_barcodePallet);
            //v_ZPL = v_ZPL + " " +  string.Format("^FO10,640^GB550,0,3^FS");
            v_ZPL = v_ZPL + " " +  string.Format("^FO10,650^A0N,20,20^FDGATE IN DATE^FS");
            v_ZPL = v_ZPL + " " +  string.Format("^CFA,20");
            v_ZPL = v_ZPL + " " +  string.Format("^FO250,650^FD{0}^FS", _gateInDate);
            //v_ZPL = v_ZPL + " " +  string.Format("^FO10,670^GB550,0,3^FS");
            v_ZPL = v_ZPL + " " +  string.Format("^FO10,680^A0N,20,20^FDBOOKING^FS");
            v_ZPL = v_ZPL + " " +  string.Format("^CFA,20");
            v_ZPL = v_ZPL + " " +  string.Format("^FO250,680^FD{0}^FS", _booking);
            //v_ZPL = v_ZPL + " " +  string.Format("^FO10,700^GB550,0,3^FS");
            v_ZPL = v_ZPL + " " +  string.Format("^FO10,710^A0N,20,20^FDAISV^FS");
            v_ZPL = v_ZPL + " " +  string.Format("^CFA,20");
            v_ZPL = v_ZPL + " " +  string.Format("^FO250,710^FD{0}^FS", _aisv);
            //v_ZPL = v_ZPL + " " +  string.Format("^FO10,730^GB550,0,3^FS");
            v_ZPL = v_ZPL + " " +  string.Format("^FO10,740^A0N,20,20^FDEXPORTER^FS");
            v_ZPL = v_ZPL + " " +  string.Format("^CFA,20");
            v_ZPL = v_ZPL + " " +  string.Format("^FO250,740^FD{0}^FS", _exporter.Length>15?_exporter.Substring(0,15): _exporter);
            //v_ZPL = v_ZPL + " " +  string.Format("^FO10,760^GB550,0,3^FS");
            v_ZPL = v_ZPL + " " +  string.Format("^FO10,770^A0N,20,20^FDVESSEL^FS");
            v_ZPL = v_ZPL + " " +  string.Format("^CFA,20");
            v_ZPL = v_ZPL + " " +  string.Format("^FO250,770^FD{0}^FS", _vessel.Length > 11? _vessel.Substring(0, 11): _vessel);
            //v_ZPL = v_ZPL + " " +  string.Format("^FO10,790^GB550,0,3^FS");
            v_ZPL = v_ZPL + " " +  string.Format("^FO10,800^A0N,20,20^FDMODALITY^FS");
            v_ZPL = v_ZPL + " " +  string.Format("^CFA,20");
            v_ZPL = v_ZPL + " " +  string.Format("^FO250,800^FD{0}^FS", _modality);
            //v_ZPL = v_ZPL + " " +  string.Format("^FO10,820^GB550,0,3^FS");
            v_ZPL = v_ZPL + " " +  string.Format("^FO10,830^A0N,20,20^FDDAE (CUSTOMS)^FS");
            v_ZPL = v_ZPL + " " +  string.Format("^CFA,20");
            v_ZPL = v_ZPL + " " +  string.Format("^FO250,830^FD{0}^FS", _dae);
            //v_ZPL = v_ZPL + " " +  string.Format("^FO10,850^GB550,0,3^FS");
            v_ZPL = v_ZPL + " " +  string.Format("^FO10,860^A0N,20,20^FDBOXES QTY^FS");
            v_ZPL = v_ZPL + " " +  string.Format("^CFA,20");
            v_ZPL = v_ZPL + " " +  string.Format("^FO250,860^FD{0}^FS", _box);
            //v_ZPL = v_ZPL + " " +  string.Format("^FO10,880^GB550,0,3^FS");
            v_ZPL = v_ZPL + " " +  "^XZ";

            return v_ZPL;
        }

        #region "EXAMPLE PRINT ZEBRA"
        public async void imprimir(string textToPrint)
        {
            Connection connection = null;

            try
            {
                BluetoothAdapter bluetoothAdapter = BluetoothAdapter.DefaultAdapter;

                if (bluetoothAdapter == null || !bluetoothAdapter.IsEnabled)
                {
                    // Bluetooth no está disponible o no está habilitado
                    return;
                }
                
                var pairedDevices = bluetoothAdapter.BondedDevices;
                if (pairedDevices != null && pairedDevices.Count > 0)
                {
                    foreach (var device1 in pairedDevices)
                    {
                        // Aquí puedes listar los dispositivos emparejados
                        Console.WriteLine(device1.Name + " - " + device1.Address);

                        if (device1.Name.Contains("zq230"))
                        {
                            v_MacAddressEntry = device1.Address;
                        }
                    }
                }

                connection = CreateConnection();

                if (connection == null)
                {
                    //SetInputEnabled(true);
                    return;
                }

                try
                {
                    await DisplayConnectionStatusAsync("Connecting...", Color.Goldenrod, 1500);

                    connection.Open();

                    await DisplayConnectionStatusAsync("Connected", Color.Green, 1500);
                    await DisplayConnectionStatusAsync("Determining printer language...", Color.Goldenrod, 1500);

                    PrinterLanguage printerLanguage = PrinterLanguage.ZPL; //ZebraPrinterFactory.GetInstance(connection).PrinterControlLanguage;
                    await DisplayConnectionStatusAsync("Printer language: " + printerLanguage.ToString(), Color.Blue, 1500);

                    UpdateConnectionStatus("Sending data...", Color.Goldenrod);

                    connection.Write(GetTestLabelBytes(printerLanguage));

                    await Task.Delay(1000);
                }
                catch (Exception e)
                {
                    await DisplayConnectionStatusAsync($"Error: {e.Message}", Color.Red, 3000);
                }
                finally
                {
                    try
                    {
                        connection?.Close();

                        await DisplayConnectionStatusAsync("Disconnecting...", Color.Goldenrod, 1000);
                        UpdateConnectionStatus("Not connected", Color.Red);
                    }
                    catch (ConnectionException) { }
                }
            }
            catch (Exception e)
            {
                //UpdateConnectionStatus($"Error: {e.Message}", Color.Red);
            }
        }

        private Connection CreateConnection()
        {
            switch (GetSelectedConnectionType())
            {
                case ConnectionType.Network:
                    return null;// new TcpConnection(IpAddressEntry.Text, GetPortNumber(PortNumberEntry.Text));

                case ConnectionType.Bluetooth:
                    try
                    {
                        return DependencyService.Get<IConnectionManager>().GetBluetoothConnection(v_MacAddressEntry);
                    }
                    catch (NotImplementedException)
                    {
                        throw new NotImplementedException("Bluetooth connection not supported on this platform");
                    }

                case ConnectionType.UsbDirect:
                    try
                    {
                        return null;// DependencyService.Get<IConnectionManager>().GetUsbConnection(SymbolicNameEntry.Text);
                    }
                    catch (NotImplementedException)
                    {
                        throw new NotImplementedException("USB connection not supported on this platform");
                    }

                case ConnectionType.UsbDriver:
                    return null;// ((DiscoveredPrinter)UsbDriverPrinterPicker.SelectedItem)?.GetConnection();

                default:
                    throw new ArgumentNullException("No connection type selected");
            }
        }

        public enum ConnectionType
        {
            Network,
            Bluetooth,
            UsbDirect,
            UsbDriver
        }
        private ConnectionType? GetSelectedConnectionType()
        {
            string connectionType = "Bluetooth";//(string)ConnectionTypePicker.SelectedItem;
            switch (connectionType)
            {
                case "Network":
                    return ConnectionType.Network;

                case "Bluetooth":
                    return ConnectionType.Bluetooth;

                case "USB Direct":
                    return ConnectionType.UsbDirect;

                case "USB Driver":
                    return ConnectionType.UsbDriver;

                default:
                    return null;
            }
        }

        private async Task DisplayConnectionStatusAsync(string statusMessage, Color color, int displayTime)
        {
            UpdateConnectionStatus(statusMessage, color);
            await Task.Delay(displayTime);
        }

        private void UpdateConnectionStatus(string statusMessage, Color color)
        {
            Device.BeginInvokeOnMainThread(() => {
                return;

                
            });
        }

        private byte[] GetTestLabelBytes(PrinterLanguage printerLanguage)
        {
            if (printerLanguage == PrinterLanguage.ZPL)
            {
                return Encoding.UTF8.GetBytes(TestLabelZpl);
            }
            else if (printerLanguage == PrinterLanguage.CPCL || printerLanguage == PrinterLanguage.LINE_PRINT)
            {
                return null;// Encoding.UTF8.GetBytes(TestLabelCpcl);
            }
            else
            {
                throw new ZebraPrinterLanguageUnknownException();
            }
        }
        
        private const string TestLabelZpl = @"
^XA
^FO30,10^BQN,2,9
^FDQA, 1;GATE IN DATE:05/06/2024;BOOKING:ECGY4654654;AISV:A1546544456;EXPORTER:DONATELLA;VESSEL:BTS2024001-SPIRIT;MODALITY:PALLETS;DAE:S165465464468744L;BOXES:48  ^FS

^FO10,540^GB550,0,3^FS

^FO10,550^A0N,20,20^FDGATE IN DATE^FS
^CFA,20
^FO250,550^FD05/06/2024^FS

^FO10,570^GB550,0,3^FS

^FO10,580^A0N,20,20^FDBOOKING^FS
^CFA,20
^FO250,580^FDECGY4654654^FS

^FO10,600^GB550,0,3^FS

^FO10,610^A0N,20,20^FDAISV^FS
^CFA,20
^FO250,610^FDA1546544456^FS

^FO10,630^GB550,0,3^FS

^FO10,640^A0N,20,20^FDEXPORTER^FS
^CFA,20
^FO250,640^FDDONATELLA^FS

^FO10,660^GB550,0,3^FS

^FO10,670^A0N,20,20^FDVESSEL^FS
^CFA,20
^FO250,670^FDBTS2024001-SPIRIT^FS

^FO10,690^GB550,0,3^FS

^FO10,700^A0N,20,20^FDMODALITY^FS
^CFA,20
^FO250,700^FDPALLETS^FS

^FO10,720^GB550,0,3^FS

^FO10,730^A0N,20,20^FDDAE (CUSTOMS)^FS
^CFA,20
^FO250,730^FDS165465464468744L^FS

^FO10,750^GB550,0,3^FS

^FO10,760^A0N,20,20^FDBOXES QTY^FS
^CFA,20
^FO250,760^FD48^FS

^FO10,780^GB550,0,3^FS


^FO75,790^GFA,6413,6413,53,,:::::01kFE,01hH07hWF2,01k02,01hG01hX02,01gH0EX01hX02,01gG03F8W01hX02,01gG0FFEW01hX02,01g03IF8V01M01FF8hM02,01Y01KFV01M07FFEhM02,01Y07KFCU01M0JFT0F8gQ02,01X01MFU01L01JF8S0F8gQ02,01X07MFCT01L03JFCS0F8gQ02,01W01IFBF3IFT01L07F81FES0F8gQ02,01W07FFE3F07FFES01L07F007ES0F8gQ02,01V01IF83F01IF8R01L0FE007E001FO0F8003CJ07J03EV02,01V07FFE03F007FFER01L0FC003E00FFE00F9FE07FF00FF8003FE001FFC01F1FCO02,01U01IF803F001IF8Q01L0FC003F01IF00FBFF07FF03FFC00IF803FFE01F7FEO02,01U07FFE003FI07IFQ01L0F8K03IF80JF87FF07FFE01IF807IF01JFO02,01T01IF8003FJ0IFCP01K01F8K07IFC0JF87FF07IF01IFC0JF81JFO02,01T07FFEI03FJ03IFP01K01F8K07F1FC0FF1F80F80FE3F03F8FE0FE3F81FE3FO02,01S01IF8I03FK0IFCO01K01F8K0FC07E0FC0FC0F80F81F83F07E1F80FC1F81F8N02,01S07FFEJ03FK03IF8N01K01F8K0FC07E0FC0FC0F81F80F83E03E1F80FC1F81F8N02,01R01IF8J03FL0IFEN01K01F8K0F803E0F80FC0F81F00F87E03E1F007C1F01F8N02,01R07FFEK03FL03IF8M01K01F8K0F803E0F807C0F81F80F87EI01F007C1F01F8N02,01Q01IF8K03FM07FFEM01K01F8K0F803E0F807C0F81JFC7EI01F007C1F01F8N02,01Q07FFEL03FM01IF8L01K01F8K0F803E0F807C0F81JFC7CI01F007C1F01F8N02,01P01IF8L03FN07IFL01L0F8K0F803E0F807C0F81JFC7CI01F007C1F01F8N02,01P07FFEM03FN01IFCK01L0FC003F0F803E0F807C0F81F8I07EI01F007C1F01F8N02,01O01IF8M03FO07IFK01L0FC003E0F803E0F807C0F81FJ07EI01F007C1F01F8N02,01O07FFEN03FP0IFCJ01L0FE007E0F803E0F807C0F81F8I07E03E1F007C1F01F8N02,01N01IF8N03FM0MFJ01L07F00FE0FC07E0F807C0F80F80F83E07E1F80FC1F01F8N02,01N07FFEO03FM0MFJ01L07F81FC07E0FC0F807C0F80FC1F83F07E0FC1FC1F01F8N02,01M01IF8O03FM0MFJ01L03JFC07F1FC0F807C0FC0FF7F83IFC0FE3F81F01F8N02,01M07FFEP03FM0MFJ01L01JF803IF80F807C0FF07IF01IFC07IF01F01F8N02,01L01IF8P03FM0MFJ01M0JF003IF80F807C0FF03FFE00IF807IF01F01F8N02,01L07FFEQ03FM0MFJ01M07FFE001IF00F807C07F01FFC007FF003FFE01F01F8N02,01K03IF8Q03FM0MFJ01M01FF8I07FC00F807803F00FFI03FCI0FF801F00FO02,01K0IFCR03FM0MFJ01hX02,01J03IFS03FM0MFJ01hX02,01J0IFCS03FM0MFJ01hX02,0103gWF001hX02,0103gWF001M01FF8gV07C0F8L02,0103gWF001M07FFEgV07C0F8L02,0103gWF001M0JFgV07C0F8L02,0103gWF001L01JF8gU07C0F8L02,0103gWF001L03JFCgU07C0F8L02,01gG03FJ03FF8P01L07F81FEgU07C0F8L02,01gG03FJ07FF8P01L07F007EgX0F8L02,01gG03FJ0IF8P01L0FE003EN03EO07CI01CR0F8L02,01gG03FI01IF8P01L0FC003F03E03E00FFC07E01F03FF8007F1E07C03E07C0F8L02,01gG03FI01IF8P01L0FCK03E03E03IF07E03F07FFC00FF9E07C03E07C0F8L02,01gG03FI03IF8P01L0F8K03E03E03IF07E03E0IFE01IFE07C03E07C0F8L02,01gG03FI07FBF8P01K01F8K03E03E07IF83E03E0JF03IFE07C03E07C0F8L02,01gG03FI0FF3F8P01K01F8K03E03E07C1F83F03E0F83F03F9FE07C03E07C0F8L02,01gG03F001FE3F8P01K01F8K03E03E07C0F83F07C1F01F07E07E07C03E07C0F8L02,01gG03F001FE3F8P01K01F807FF03E03E0780F81F07C1F01F07E03E07C03E07C0F8L02,01gG03F003FC3F8P01K01F807FF03E03EI01F81F07CI03F07C03E07C03E07C0F8L02,01gG03F007F83F8P01K01F807FF03E03EI0FF81F8F8001FF07C03E07C03E07C0F8L02,01gG03F00FF03F8P01K01F807FF03E03E01IF80F8F803IF07C03E07C03E07C0F8L02,01gG03F01FE03F8P01K01F807FF03E03E03IF80F8F807IF07C03E07C03E07C0F8L02,01gG03F03FC03F8P01L0F8001F03E03E07FEF80F8F00FFDF07C03E07C03E07C0F8L02,01gG03F03FC03F8P01L0FC001F03E03E0FE0F807DF01FC1F07C03E07C03E07C0F8L02,01gG03F07F803F8P01L0FC003F03E03E0F80F807DF01F01F07C03E07C03E07C0F8L02,01gG03F0FF003F8P01L07E003F03E03E0F80F807DF01F01F07C03E07C07E07C0F8L02,01gG03F1FE003F8P01L07F007F03E07E0F80F803FE01F03F07E07E07E07E07C0F8L02,01gG03F3FC003F8P01L03F81FF03F07E0F81F803FE01F03F07F07E07E0FE07C0F8L02,01gG03F7F8003F8P01L03KF03IFE0FC3F803FE01F87F03IFE07IFE07C0F8L02,01gG03F7F8003F8P01L01KF03IFE0JF801FC01JF03IFE03IFE07C0F8L02,01gG03IFI03F8P01M0IFEF01IFE07IF801FC01JF01IFE03IFE07C0F8L02,01gG03FFEI03F8P01M07FF8F01FFBE07FEFC01FC00FFDF80FF9E01FF3E07C0F8L02,01gG03FFCI03F8P01M01FF0F007E3E03FC7C00F8007F0F807F3E00FE3E07C0F8L02,01gG03FF8I03F8P01gL0F8P03EX02,:01gG03FFJ03F8P01gK01FQ03EX02,01gG03NF8P01gK03FQ03EX02,01gG03NF8P01gK0FFQ03EX02,01gG03NF8P01gK0FEQ03EX02,:01gG03NF8P01gK0FCQ03FX02,01gG03F7JFBF8P01hX02,01gG03FK03F8P01hX02,:::01gG03FK03F8Q0hXF2,01k02,01kFE,,::00gPF,01gPF8,01JFE7gJF8,01FE07C3F21F1F03F83E0QF8,01FC0700E01F0F00F03E07PF8I0100204I0803807C0601J070078007004103CJ07I0E006030380100204102,01FC0701E01E0F00E01E03PF8I018070C00180FE07C1F03I01FC07E01FC06103FI01FC03F80E0707E018070C186,01FC0F00E01E0F00E01E03PF8I038070C00181830101102I030C046030C041023I030E060C0E0E063038070C08C,01FC3F18E01C0700E01E03PF8I028078800183I0103002I06I0C20606043021I06I0C0C0B0A063028078C0C8,01FC071FE01C0F00E01E03PF8I048058800102I0303802I0CI0C20C06043061I04I08040B1A04304805880D8,01FC071FE01E0F00E01E03PF8I0C8048800102I0301E06I08I08608020C2067I0C00180409120460C8048807,01FC0F19E01C0700E01E03PF8I0CC04D800106I0200706I08FE0FC08060C207CI08001804192607C0CC04D806,01FC3F18E01C0700E01E03PF8001FC0C5800306I0200306I080C0B0080408204J0C00180C19660C01FC0C5806,01FC0F00E01C0300E01E03PF80010C0C7800302I0206304I0C0C0900C0C08604J0C00181819C40C010C0C7804,01FC0701F0380700F01E03PF800304083I02030E0606204I0E181980E180C40CJ06180C3011C4080304083004,01FC0701F01C0300F03E03PF800204083I0201F80603E04I07F018C07F00FC0CJ03F007E01084080204083004,01FE07C3F878E783F07E63PF8T06J018L08L08002N08001,01MFCgHF8,01gPF8,,::::::^FS
^XZ



";
    }
#endregion

}

   


