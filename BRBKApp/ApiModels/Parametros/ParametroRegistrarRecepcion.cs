using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiModels.Parametros
{
    public class ParametroRegistrarRecepcion : Base
    {
        public long idRecepcion { get; set; }
        public long idTarjaDet { get; set; }
        public string lugar { get; set; }
        public int cantidad { get; set; }
        public string estado { get; set; }
        public List<ParametroRegistrarRecepcionFoto> Fotos { get; set; }
    }

    public class ParametroRegistrarRecepcionFoto : Base
    {
        public byte[] foto { get; set; }
        public string ruta { get; set; }
        public string estado { get; set; }
    }

    public class ParametroActualizarRecepcion : Base
    {
        public Int64? IdRecepcion { get; set; }
        public long idTarjaDet { get; set; }
        public int cantidad { get; set; }
        public string ubicacion { get; set; }
        public string observacion { get; set; }
        public string estado { get; set; }
    }
}
