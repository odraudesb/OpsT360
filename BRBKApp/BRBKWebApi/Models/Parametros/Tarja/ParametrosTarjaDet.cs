using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MiWebApi
{
    public class ParametrosTarjaDet
    {
        public class ParametrosConsultaListaBL : Base
        {
            public string MRN { get; set; }
            public string Lugar { get; set; }

            public int? PreValidationsTransaction(out string msg)
            {
                if (string.IsNullOrEmpty(this.MRN))
                {
                    msg = "Especifique el MRN";
                    return 0;
                }

                if (string.IsNullOrEmpty(this.Lugar))
                {
                    this.Lugar = "BODEGA";
                }

                msg = string.Empty;
                return 1;
            }
        }

        public class ParametrosConsultaGetBL : Base
        {
            public long idTarjaDet { get; set; }

            public int? PreValidationsTransaction(out string msg)
            {
                if (idTarjaDet == 0)
                {
                    msg = "Especifique el Id del BL";
                    return 0;
                }

                msg = string.Empty;
                return 1;
            }
        }
    }
}