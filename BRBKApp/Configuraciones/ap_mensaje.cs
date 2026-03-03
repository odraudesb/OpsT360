using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Configuraciones
{
    public class ap_mensaje
    {
        public string codigo { get; set; }
        public string mensaje_pantalla { get; set; }
        public string opcion_posible { get; set; }
        public string descripcion_general { get; set; }

        public ap_mensaje(string _cd, string _mn, string _op, string _dg)
        {
            this.codigo = _cd;
            this.mensaje_pantalla = _mn;
            this.opcion_posible = _op;
            this.descripcion_general = _dg;
        }
        public ap_mensaje()
        {

        }
    }
}
