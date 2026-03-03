using ApiModels.AppModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace BRBKApp.Services
{
    public static class NavigationService
    {
        public static VHSTarjaMensaje TarjaModelToDetailPage { get; set; }
        public static VHSTarjaMensaje AddDetailTarja { get; set; }

        // Used for navigating to CEDI tarja detail pages
        public static CediTarjaMensaje CediTarjaModelToDetailPage { get; set; }
    }
}
