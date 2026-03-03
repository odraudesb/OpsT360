using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiModels.Parametros
{
    public class ParametroVHSActualizarDetalleTarja
    {
        public long DetalleTarjaId { get; set; }
        public int BloqueId { get; set; }
        public int NumeroBloque { get; set; }
        public string VIN { get; set; }
        public string Observaciones { get; set; }
    }
}
