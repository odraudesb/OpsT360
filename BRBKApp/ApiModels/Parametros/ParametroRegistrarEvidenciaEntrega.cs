using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiModels.Parametros
{
    public class ParametroRegistrarEvidenciaEntrega
    {
        public long VehiculoDespachadoID { get; set; }
        public string Observacion { get; set; }
        public string Usuario { get; set; }
        public byte[] Foto { get; set; }
    }

}
