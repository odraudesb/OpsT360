using ApiModels.AppModels;
using System.Collections.Generic;

namespace ApiModels.Parametros
{
    public class ParametroCediCrearEvidenciaEntrega
    {
        public long VehiculoDespachadoID { get; set; }
        public string Observacion { get; set; }
        public string Usuario { get; set; }
        public List<CediEvidenciaEntregaFoto> Fotos { get; set; }
    }
}
