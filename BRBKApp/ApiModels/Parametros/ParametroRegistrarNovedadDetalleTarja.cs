using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiModels.Parametros
{
    public class ParametroRegistrarNovedadDetalleTarja
    {
        public long DetalleTarjaID { get; set; }
        public string Descripcion { get; set; }
        public string Usuario { get; set; }

        public int TipoNovedadID { get; set; }

        public List<ParametroRegistrarNovedadDetalleTarjaFoto> Fotos { get; set; }
    }

    public class ParametroRegistrarNovedadDetalleTarjaFoto
    {
        public byte[] ArrayFoto { get; set; }
        public int Orden { get; set; }
    }

}
