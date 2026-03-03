
using System;

using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiModels.AppModels
{
    public class CediOrdenTrabajo : Base
    {
        public int OrdenTrabajoId { get; set; }
        public string NumeroOrden { get; set; }
        public string Mensaje { get; set; }
    }
}
