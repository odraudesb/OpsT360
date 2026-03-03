using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiModels.Parametros
{
    public class ParametroRegistrarDespacho : Base
    {
        public long idDespacho { get; set; }
        public long idTarjaDet { get; set; }

        public string pase { get; set; }
        public string mrn { get; set; }
        public string msn { get; set; }
        public string hsn { get; set; }
        public string placa { get; set; }
        public string idchofer { get; set; }
        public string chofer { get; set; }
        public decimal cantidad { get; set; }
        public string observacion { get; set; }
        public string estado { get; set; }
        public string delivery { get; set; }
        public long PRE_GATE_ID  { get; set; }
    public List<ParametroRegistrarDespachoFoto> Fotos { get; set; }
    }

    public class ParametroRegistrarDespachoFoto : Base
    {
        public byte[] foto { get; set; }
        public string ruta { get; set; }
        public string estado { get; set; }
    }

    public class ParametroRegistrarDespachoVBS : Base
    {
        public string idNave { get; set; }
        public int idExportador { get; set; }
        public int idBodega { get; set; }
        public int idBloque { get; set; }
        public int cantidadPalets { get; set; }
        public int cantidadBox { get; set; }
        public int arrastre { get; set; }
        public int pendiente { get; set; }
        public string estado { get; set; }
        public string booking { get; set; }
    }

    public class ParametroAnularDespachoVBS : Base
    {
        public long idOrdenDespacho { get; set; }
        public string estado { get; set; }
    }

    public class ParametroRegistrarPreDespachoVBS : Base
    {
        public long idMovimiento { get; set; }
    }
}
