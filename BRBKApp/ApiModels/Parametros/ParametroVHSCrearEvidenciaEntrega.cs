using ApiModels.AppModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiModels.Parametros
{
    public class ParametroVHSCrearEvidenciaEntrega
    {
        public long VehiculoDespachadoID { get; set; }
        public string Observacion { get; set; }
        public string Usuario { get; set; }
        public List<VHSEvidenciaEntregaFoto> Fotos { get; set; }
    }

}
