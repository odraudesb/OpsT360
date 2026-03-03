using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiModels.Parametros
{
    public class ParametroVHSListaTarjaPendiente : Base
    {
        public bool Filtrar { get; set; }
        public int OrdenTrabajoId { get; set; }
    }
}
