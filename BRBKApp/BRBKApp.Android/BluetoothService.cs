using Android.Bluetooth;
using BRBKApp.Droid;
using BRBKApp.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;

[assembly: Dependency(typeof(BluetoothService))]
namespace BRBKApp.Droid
{
    public class  BluetoothService //: IBluetoothService
    {
        public Task<List<string>> GetBondedDevicesAsync()
        {
            var bondedDevices = new List<string>();
            var adapter = BluetoothAdapter.DefaultAdapter;

            if (adapter == null)
            {
                throw new Exception("No Bluetooth adapter found.");
            }

            if (!adapter.IsEnabled)
            {
                throw new Exception("Bluetooth adapter is not enabled.");
            }

            foreach (var device in adapter.BondedDevices)
            {
                bondedDevices.Add($"{device.Name} - {device.Address}");
            }

            return Task.FromResult(bondedDevices);
        }
    }
}