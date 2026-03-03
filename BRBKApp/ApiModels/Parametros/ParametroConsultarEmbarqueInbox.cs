using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiModels.Parametros
{
    public class ParametroConsultarEmbarqueInbox
    {
        public string idNave { get; set; }
        public string idExportador { get; set; }
    }

    public class ParametroConsultarEmbarqueCab
    {
        public long id { get; set; }
    }

    public class ParametroConsultarEmbarqueMovimiento
    {
        public long id { get; set; }
    }
    public class ParametroRegistrarRecepcionEmbarque : Base
    {
        public long? idEmbarqueMovimiento { get; set; }
        public long? idEmbarqueCab { get; set; }
        public string codigoCaja { get; set; }
        public int? idHold { get; set; }
        public int? idPiso { get; set; }
        public int? idMarca { get; set; }
        public int? idModalidad { get; set; }
        public int? box { get; set; }
        public string tipo { get; set; }
        public int? idtipoMovimiento { get; set; }
        public string comentario { get; set; }
        public int? fechaProceso { get; set; }
        public string estado { get; set; }
        public string usuarioCrea { get; set; }

        public List<ParametroRegistrarRecepcionFotoEmbarque> Fotos { get; set; }
    }

    public class ParametroRegistrarRecepcionFotoEmbarque : Base
    {
        public byte[] foto { get; set; }
        public string ruta { get; set; }
        public string estado { get; set; }

    }


}
