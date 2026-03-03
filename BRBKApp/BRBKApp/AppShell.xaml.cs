using BRBKApp.ViewModels;
using BRBKApp.Views;
using ApiModels.AppModels;
using System;
using System.Collections.Generic;
using System.Windows.Input;
using Xamarin.Forms;

namespace BRBKApp
{
    public partial class AppShell : Xamarin.Forms.Shell
    {
        public ICommand AbrirTarjaNovedades { get; }
        public ICommand AbrirCediNovedades { get; }
        public AppShell()
        {
            InitializeComponent();
            BindingContext = new ShellViewModel();
            Routing.RegisterRoute(nameof(HomePage), typeof(HomePage));
            Routing.RegisterRoute(nameof(InboxPage), typeof(InboxPage));
            Routing.RegisterRoute(nameof(EditDetail), typeof(EditDetail));
            Routing.RegisterRoute(nameof(UpdateDetail), typeof(UpdateDetail));
            Routing.RegisterRoute(nameof(StartupPage), typeof(StartupPage));
            Routing.RegisterRoute(nameof(TaskDetails), typeof(TaskDetails));
            Routing.RegisterRoute(nameof(AlertReportPage), typeof(AlertReportPage));
            Routing.RegisterRoute(nameof(DispatchPage), typeof(DispatchPage));
            Routing.RegisterRoute(nameof(DownloadConfirmationPage), typeof(DownloadConfirmationPage));
            Routing.RegisterRoute(nameof(MtySealValidationPage), typeof(MtySealValidationPage));
            Routing.RegisterRoute(nameof(MtySealRegistersPage), typeof(MtySealRegistersPage));
            Routing.RegisterRoute(nameof(FclSealPreEmbarquePage), typeof(FclSealPreEmbarquePage));
            Routing.RegisterRoute(nameof(FclSealAforoPage), typeof(FclSealAforoPage));
            Routing.RegisterRoute(nameof(FclSealAssignsPage), typeof(FclSealAssignsPage));
            Routing.RegisterRoute(nameof(FclSealYardValidationPage), typeof(FclSealYardValidationPage));
            Routing.RegisterRoute(nameof(MtyPOWPage), typeof(MtyPOWPage));

            Routing.RegisterRoute(nameof(VBSInboxAisvPage), typeof(VBSInboxAisvPage));
            Routing.RegisterRoute(nameof(VBSNewDetail), typeof(VBSNewDetail));
            Routing.RegisterRoute(nameof(VBSTaskDetails), typeof(VBSTaskDetails));
            Routing.RegisterRoute(nameof(VBSAssignsPositionPage), typeof(VBSAssignsPositionPage));
            Routing.RegisterRoute(nameof(VBSCheckLoadPage), typeof(VBSCheckLoadPage));

            Routing.RegisterRoute(nameof(VBSOrderDispatchInboxPage), typeof(VBSOrderDispatchInboxPage));
            Routing.RegisterRoute(nameof(VBSOrderDispatchNewOrder), typeof(VBSOrderDispatchNewOrder));
            Routing.RegisterRoute(nameof(VBSOrderDispatchDetailsPage), typeof(VBSOrderDispatchDetailsPage));

            Routing.RegisterRoute(nameof(VBSPreDispatchInboxPage), typeof(VBSPreDispatchInboxPage));
            Routing.RegisterRoute(nameof(VBSPreDispatchDetailsPage), typeof(VBSPreDispatchDetailsPage));
            Routing.RegisterRoute(nameof(VBSPreDispatchProcessPage), typeof(VBSPreDispatchProcessPage));

            Routing.RegisterRoute(nameof(VBSDispatchPage), typeof(VBSDispatchPage));
            Routing.RegisterRoute(nameof(VBSShipmentInboxPage), typeof(VBSShipmentInboxPage));
            Routing.RegisterRoute(nameof(VBSShipmentInboxNewPage), typeof(VBSShipmentInboxNewPage));
            Routing.RegisterRoute(nameof(VBSShipmentDetailsPage), typeof(VBSShipmentDetailsPage));
            Routing.RegisterRoute(nameof(VBSShipmentDetailsNewPage), typeof(VBSShipmentDetailsNewPage));
            Routing.RegisterRoute(nameof(VBSAddAisvExternalPage), typeof(VBSAddAisvExternalPage));

            Routing.RegisterRoute(nameof(VHSOrdenTrabajoPage), typeof(VHSOrdenTrabajoPage));
            Routing.RegisterRoute(nameof(VHSTarjaPage), typeof(VHSTarjaPage));
            Routing.RegisterRoute(nameof(VHSTarjaDetallePage), typeof(VHSTarjaDetallePage));
            Routing.RegisterRoute(nameof(VHSContenedorPage), typeof(VHSContenedorPage));
            Routing.RegisterRoute(nameof(VHSTarjaConsultaDetail), typeof(VHSTarjaConsultaDetail));
            
            //Routing.RegisterRoute(nameof(VHSNovedadDetalleTarjaPage), typeof(VHSNovedadDetalleTarjaPage));

            Routing.RegisterRoute(nameof(CediOrdenTrabajoPage), typeof(CediOrdenTrabajoPage));
            Routing.RegisterRoute(nameof(CediTarjaPage), typeof(CediTarjaPage));
            Routing.RegisterRoute(nameof(CediTarjaDetallePage), typeof(CediTarjaDetallePage));
            Routing.RegisterRoute(nameof(CediTarjaConsultaDetail), typeof(CediTarjaConsultaDetail));
            Routing.RegisterRoute(nameof(CediVehiculosDespachoPage), typeof(CediVehiculosDespachoPage));

            //AbrirTarjaNovedades = new Command(async () =>
            //{
            //    await Shell.Current.GoToAsync("VHSTarjaConsultaDetail?origen=Novedad");
            //});
            //AbrirTarjaNovedades = new Command(async () =>
            //{

            //    await Shell.Current.GoToAsync("//TarjaNovedadesRoot?origen=Novedad");

            //});
            var vm = (ShellViewModel)this.BindingContext;

            if (vm.esActivoCEDI5)   // ← tu permiso
            {
                this.Items.Add(new MenuItem
                {
                    Text = "      >> [CEDI] Novedades XXXXX",
                    Command = vm.AbrirCediNovedades
                });
            }

            AbrirCediNovedades = new Command(async () =>
            {
                await Shell.Current.GoToAsync("CediTarjaConsultaDetail?origen=Novedad");
            });





        }

        private async void OnMenuItemClicked(object sender, EventArgs e)
        {
            App.Current.Properties["IsLoggedIn"] = false;
            App.Current.Properties["UserId"] = null;
            await Xamarin.Essentials.SecureStorage.SetAsync("isLogged", "0");
            await Shell.Current.GoToAsync("//LoginPage");
        }
    }
}
