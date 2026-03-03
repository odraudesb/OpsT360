using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlConexion
{
    class Cls_Parametros
    {

        public string aplicacion { get; set; }
        public string modulo { get; set; }
        public string nombre { get; set; }
        public string valor { get; set; }
        public string tipodato { get; set; }
        public Cls_Parametros()
        {

        }
        public Cls_Parametros(string _aplicacion, string _modulo, string _nombre, string _valor)
        {
            this.aplicacion = _aplicacion;
            this.modulo = _modulo;
            this.nombre = _nombre;
            this.valor = _valor;
        }

    }


}
