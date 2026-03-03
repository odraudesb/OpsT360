using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiModels.Parametros
{
   
        public class ParametroRegistrarEmbarqueVBS : Base
        {
            public long? idEmbarqueCab { get; set; }
            public string barcode { get; set; }
            public string idNave { get; set; }
            public string nave { get; set; }
            public string idExportador { get; set; }
            public string Exportador { get; set; }
            public string estado { get; set; }
        }
    public class ParametroRegistrarAisvExternoVBS : Base
    {
        public string idNave { get; set; }
        public int idHold { get; set; }
        public int idBodega { get; set; }
        public int idBloque { get; set; }
        public string aisv { get; set; }
    }
}
