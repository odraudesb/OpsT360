using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiModels.AppModels
{
    public class VHSTarjaDetalleFoto
    {
        public int FotoID { get; set; }
        public int DetalleTarjaID { get; set; }
        public string FotosVehiculo { get; set; }
        public byte[] ArrayFoto { get; set; }
        public int Orden {  get; set; }
    }
}
