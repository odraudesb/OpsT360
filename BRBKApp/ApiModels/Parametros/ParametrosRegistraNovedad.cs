using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiModels.Parametros
{
    public class ParametroRegistrarNovedad : Base
    {
        public long idNovedad { get; set; }
        public long idRecepcion { get; set; }

        public string descripcion { get; set; }

        public string estado { get; set; }

        public List<ParametroRegistrarNovedadFoto> Fotos { get; set; }
    }

    public class ParametroRegistrarNovedadFoto : Base
    {
        public byte[] foto { get; set; }
        public string ruta { get; set; }
        public string estado { get; set; }
    }

}
