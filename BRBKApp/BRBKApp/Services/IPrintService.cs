using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Zebra.Sdk.Comm;
using Zebra.Sdk.Printer.Discovery;

namespace BRBKApp.Services
{
    public interface IPrintService
    {
        void Print(string textToPrint);
    }
}

