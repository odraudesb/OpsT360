using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiModels.AppModels
{
    public class VHSTarjaModel
    {
        public int TarjaId { get; set; }
        public int OrdenTrabajoId { get; set; }
        public List<VHSTarjaFotos> TarjaFotos { get; set; }
        public string Contenido { get; set; }
        public object Observacion { get; set; }
        public string Usuario { get; set; }
        
    }
}
