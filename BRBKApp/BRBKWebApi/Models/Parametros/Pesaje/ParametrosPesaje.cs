using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MiWebApi
{
    public class ParametrosPesaje
    {
        public class ParametrosRegistraPeso: Base
        {
            public long gkey { get; set; }
            public string container { get; set; }
            public string peso { get; set; }
            public bool estado { get; set; }
            public string ip { get; set; }
            public string mensaje { get; set; }


            public int? PreValidationsTransaction(out string msg)
            {
                if (string.IsNullOrEmpty(this.container))
                {
                    msg = "Ingrese el numero del contenedor";
                    return 0;
                }

                if (string.IsNullOrEmpty(this.peso))
                {
                    msg = "Ingrese el peso";
                    return 0;
                }

                if (string.IsNullOrEmpty(this.ip))
                {
                    msg = "Ingrese la IP";
                    return 0;
                }

                if (string.IsNullOrEmpty(this.Create_user))
                {
                    msg = "Debe especificar el Create_user responsable";
                    return 0;
                }

                msg = string.Empty;
                return 1;
            }
        }
    }
}
