using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MiWebApi
{
    public class ParametrosRecepcion
    {
        public class ParametrosConsultaListaRecepcion : Base
        {
            public long idTarjaDet { get; set; }
            public string lugar { get; set; }

            public int? PreValidationsTransaction(out string msg)
            {
                if (this.idTarjaDet <=0)
                {
                    msg = "Seleccione un BL";
                    return 0;
                }

                if (this.lugar == string.Empty)
                {
                    //msg = "Indique el lugar de la transacción";
                    //return 0;
                    this.lugar = "MUELLE";
                }

                msg = string.Empty;
                return 1;
            }
        }

        public class ParametrosConsultaRecepcion : Base
        {
            public Int64? Id { get; set; }

            public int? PreValidationsTransaction(out string msg)
            {
                if (!this.Id.HasValue)
                {
                    msg = "Especifique el código de la recepción";
                    return 0;
                }

                msg = string.Empty;
                return 1;
            }
        }

        public class ParametrosRegistraRecepcion : Base
        {
            public long idTarjaDet { get; set; }

            public int cantidad { get; set; }

            public string lugar { get; set; }
            public string estado { get; set; }
            public List<ParametrosRegistraFotoRecepcion> Fotos { get; set; }

            public int? PreValidationsTransaction(out string msg)
            {
                if (this.idTarjaDet <= 0)
                {
                    msg = "Seleccione un BL";
                    return 0;
                }

                if (cantidad == 0)
                {
                    msg = "Debe especificar la cantidad";
                    return 0;
                }

                if (string.IsNullOrEmpty(this.lugar))
                {
                    this.lugar = "MUELLE";
                    //msg = "Debe especificar el lugar";
                    //return 0;
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

                if (Fotos.Count ==0)
                {
                    msg = "Debe agregar al menos una foto";
                    return 0;
                }

                foreach (ParametrosRegistraFotoRecepcion oParametroFotos in Fotos)
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

        public class ParametrosActualizaRecepcion : Base
        {
            public Int64? IdRecepcion { get; set; }
            public long idTarjaDet { get; set; }
            public int cantidad { get; set; }
            public string ubicacion { get; set; }
            public string observacion { get; set; }
            public string estado { get; set; }

            public int? PreValidationsTransaction(out string msg)
            {
                if (this.IdRecepcion <= 0)
                {
                    msg = "Seleccione una recepción";
                    return 0;
                }

                if (this.idTarjaDet <= 0)
                {
                    msg = "Seleccione un BL";
                    return 0;
                }

                if (cantidad == 0)
                {
                    msg = "Debe especificar la cantidad";
                    return 0;
                }

                if (string.IsNullOrEmpty(this.ubicacion))
                {
                    msg = "Debe especificar la ubicación ";
                    return 0;
                }

                if (string.IsNullOrEmpty(this.observacion))
                {
                    msg = "Debe especificar una observación";
                    return 0;
                }

                if (string.IsNullOrEmpty(this.estado))
                {
                    msg = "Debe especificar el estado";
                    return 0;
                }

                if (string.IsNullOrEmpty(this.Modifie_user))
                {
                    msg = "Debe especificar el Create_user responsable";
                    return 0;
                }

                

                msg = string.Empty;
                return 1;
            }

        }

        public class ParametrosRegistraFotoRecepcion : Base
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


