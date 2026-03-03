using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiModels.Parametros
{
    public class ParametroConsultarCheckLoad
    {
        public string idNave { get; set; }
        public int? idBodega { get; set; }
        public int? idBloque { get; set; }
        public int? idExportador { get; set; }
        public string booking { get; set; }
        public string barcode { get; set; }
    }

    public class ParametroConsultarOrdenDespacho
    {
        public string idNave { get; set; }
        public int? idExportador { get; set; }
        public int? idBloque { get; set; }
        public string booking { get; set; }
    }

    public class ParametroConsultarListaExportador
    {
        public string idNave { get; set; }
        public int? idBodega { get; set; }
    }

    public class ParametroConsultarSlotDisponible
    {
        public int idBodega { get; set; }
        public int idBloque { get; set; }
        public int idFila { get; set; }
        public int idAltura { get; set; }
    }
}
