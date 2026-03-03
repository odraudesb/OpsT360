using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiModels.Parametros
{
    public class ParametroConsultarUbicacionPorBarcode
    {
        public string barcode { get; set; }
    }

    public class ParametroConsultarListaBodegaXNave
    {
        public string idNave { get; set; }
    }
}
