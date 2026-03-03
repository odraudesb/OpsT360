using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiModels.Parametros
{
    public class ParametroVHSTarjaDetalleUpdate
    {
        public long DetalleTarjaID { get; set; }
        public int BloqueId { get; set; }
        public int NumeroBloque { get; set; }
        public string DocumentoTransporte { get; set; }
        public string PackingList { get; set; }
        public string VIN { get; set; }
        public string NumeroMotor { get; set; }
        public string Observaciones { get; set; }
    }
}
