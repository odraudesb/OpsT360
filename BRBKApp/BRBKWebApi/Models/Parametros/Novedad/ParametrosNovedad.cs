using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MiWebApi
{
    public class ParametrosNovedad
    {

        public class ParametrosRegistraNovedad : Base
        {
            public long idRecepcion { get; set; }
           
            public string descripcion { get; set; }

            public string estado { get; set; }

           public List<ParametrosRegistraFotoNovedad> Fotos { get; set; }

            public int? PreValidationsTransaction(out string msg)
            {
                if (this.idRecepcion <= 0)
                {
                    msg = "Seleccione una recepción";
                    return 0;
                }

                if (string.IsNullOrEmpty(this.descripcion))
                {
                    msg = "Debe describir la novedad";
                    return 0;
                }

                if (string.IsNullOrEmpty(this.estado))
                {
                    msg = "Debe especificar el estado";
                    return 0;
                }

                if (string.IsNullOrEmpty(this.Create_user))
                {
                    msg = "Debe especificar el Create_user responsable";
                    return 0;
                }

                string oError = string.Empty;

                if (Fotos == null)
                {
                    msg = "Debe agregar al menos una foto";
                    return 0;
                }

                if (Fotos.Count == 0)
                {
                    msg = "Debe agregar al menos una foto";
                    return 0;
                }

                foreach (ParametrosRegistraFotoNovedad oParametroFotos in Fotos)
                {
                    if (oParametroFotos.PreValidationsTransaction(out oError) == 0)
                    {
                        msg = oError;
                        break;
                    }
                }

                if (!string.IsNullOrEmpty(oError))
                {
                    msg = oError;
                    return 0;
                }

                msg = string.Empty;
                return 1;
            }
        }

        public class ParametrosRegistraFotoNovedad : Base
        {
            public byte[] foto { get; set; }
            public string ruta { get; set; }
            public string estado { get; set; }

            public int? PreValidationsTransaction(out string msg)
            {
                if (string.IsNullOrEmpty(this.estado))
                {
                    msg = "Debe especificar el estado";
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


