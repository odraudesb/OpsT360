using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using BRBKApp.Droid;
using BRBKApp.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Xamarin.Forms;

[assembly: Dependency(typeof(LocalFileHelper))]
namespace BRBKApp.Droid
{
    public class LocalFileHelper : ILocHelper
    {

        public string GetLocalFilePath(string filename)
        {
            string docFolder = System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData);

            string libFolder = Path.Combine(docFolder);

            return Path.Combine(libFolder, filename);

        }

    }
}