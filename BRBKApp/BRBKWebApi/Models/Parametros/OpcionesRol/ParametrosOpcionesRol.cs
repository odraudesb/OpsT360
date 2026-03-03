using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MiWebApi
{
    public class ParametrosOpcionesRol
    {
        public class ParametrosConsultaListaOpcionesRol : Base
        {
            public long idRol { get; set; }

            public int? PreValidationsTransaction(out string msg)
            {
                if (this.idRol <= 0)
                {
                    msg = "Seleccione un idRol";
                    return 0;
                }

                msg = string.Empty;
                return 1;
            }
        }

    }
}