using ApiModels.AppModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiModels.Parametros
{
    public class ParametroVHSCreaTarja : Base
    {
        public int OrdenId { get; set; }
        public string NumeroOrden { get; set; }
        public string Contenido { get; set; }
        public string Observaciones { get; set; }
        public List<VHSTarjaFotos> Fotos { get; set; }
    }
}
