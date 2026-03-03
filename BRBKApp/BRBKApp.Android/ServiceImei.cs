using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Telephony;
using Android.Views;
using Android.Widget;
using BRBKApp.Droid;
using BRBKApp.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xamarin.Forms;
using static Android.Provider.Settings;

[assembly: Dependency(typeof(ServiceImei))]
namespace BRBKApp.Droid
{
    public class ServiceImei : IServiceImei
    {
        public string GetImei()
        {
            try
            {
                var context = Android.App.Application.Context;
                string id = Secure.GetString(context.ContentResolver, Secure.AndroidId);
                return id;
            }
            catch (Exception e)
            {
                return null;
            }
        }
    }
}