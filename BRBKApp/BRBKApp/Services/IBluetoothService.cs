using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BRBKApp.Services
{
    public interface IBluetoothService
    {
        Task<List<string>> GetBondedDevicesAsync();
    }
}
