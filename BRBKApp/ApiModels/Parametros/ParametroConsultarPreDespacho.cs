using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiModels.Parametros
{
    public class ParametroConsultarPreDespacho
    {
        public int? idBodega { get; set; }
    }
    public class ParametroConsultarFilasPreDespacho
    {
        public long idOrdenDespacho { get; set; }
    }

    public class ParametroConsultarMovimientosFilas
    {
        public long idOrdenDespacho { get; set; }
        public int idFila { get; set; }
    }

    public class ParametroConsultarAisvPorBooking
    {
        public string booking { get; set; }
    }
}
