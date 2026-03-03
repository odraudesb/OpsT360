using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MiWebApi
{
    public class ParametrosPallet
    {
        public class ParametrosPaletizado : Base
        {
            public string numero_carga { get; set; }
            public string contenedor { get; set; }

            public string usuario { get; set; }

            public int cantidad { get; set; }

            public int? PreValidationsTransaction(out string msg)
            {

                if (string.IsNullOrEmpty(this.numero_carga))
                {
                    msg = "Debe ingresar el número de carga BL";
                    return 0;
                }
                if (string.IsNullOrEmpty(this.contenedor))
                {
                    msg = "Debe ingresar el número de contenedor";
                    return 0;
                }

                if (cantidad == 0) 
                {
                    msg = "Debe ingresar la cantidad de pallet";
                    return 0;
                }

                msg = string.Empty;
                return 1;
            }
        }
    }
}