using Android.App;
using Android.Net.Wifi;
using BRBKApp.Droid;
using BRBKApp.Services;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using Xamarin.Forms;

[assembly: Dependency(typeof(NetService))]
namespace BRBKApp.Droid
{
    class NetService : INetServices
    {
        public string ConvertHostIP()
        {
            WifiManager wifiManager = (WifiManager)Android.App.Application.Context.GetSystemService(Service.WifiService);
            int ip = wifiManager.ConnectionInfo.IpAddress;
            IPAddress ipAddr = new IPAddress(ip);

            //  System.out.println(host);
            return ipAddr.ToString();
        }
    }
}