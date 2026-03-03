using ApiModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BRBKWebApi.Models.Parametros.Embarque
{
    public class ParametrosEmbarque
    {
        public class ParametrosConsultaListaInboxEmbarque
        {
            public string idNave { get; set; }
            public string idExportador { get; set; }

            public int? PreValidationsTransaction(out string msg)
            {
                if ((string.IsNullOrEmpty(idNave)))
                {
                    msg = "Seleccione la nave";
                    return 0;
                }

                msg = string.Empty;
                return 1;
            }
        }
        public class ParametrosRegistrarEmbarque : Base
        {
            public long? idEmbarqueCab { get; set; }
            public string barcode { get; set; }
            public string idNave { get; set; }
            public string nave { get; set; }
            public string idExportador { get; set; }
            public string Exportador { get; set; }
            public string estado { get; set; }


            public int? PreValidationsTransaction(out string msg)
            {
                if (string.IsNullOrEmpty(this.idNave))
                {
                    msg = "Debe especificar la nave";
                    return 0;
                }

                if (string.IsNullOrEmpty(this.idExportador))
                {
                    msg = "Debe especificar el exportador";
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

                msg = string.Empty;
                return 1;
            }
        }
        public class ParametrosConsultarEmbarque
        {
            public long id { get; set; }

            public int? PreValidationsTransaction(out string msg)
            {
                if (id == 0)
                {
                    msg = "Especifique el Id del Embarque";
                    return 0;
                }

                msg = string.Empty;
                return 1;
            }
        }

        public class ParametrosRegistrarMovimiento : Base
        {
            public long? idEmbarqueMovimiento { get; set; }
            public long? idEmbarqueCab { get; set; }
            public string origen { get; set; }
            public string codigoCaja { get; set; }
            public int? idHold { get; set; }
            public int? idPiso { get; set; }
            public int? idMarca { get; set; }
            public int? idModalidad { get; set; }
            public int? box { get; set; }
            public string tipo { get; set; }
            public int? idtipoMovimiento { get; set; }
            public string comentario { get; set; }
            public int? fechaProceso { get; set; }
            public string estado { get; set; }

            public List<ParametrosRegistraFotoMovimientoEmbarqueVBS> Fotos { get; set; }
            public int? PreValidationsTransaction(out string msg)
            {
                if (string.IsNullOrEmpty(this.estado))
                {
                    msg = "Debe especificar el estado";
                    return 0;
                }

                if (this.estado == "ANU")
                {
                    msg = string.Empty;
                    return 1;
                }

                if (string.IsNullOrEmpty(this.codigoCaja))
                {
                    msg = "Debe especificar el codigo de caja";
                    return 0;
                }

                if (string.IsNullOrEmpty(this.tipo))
                {
                    msg = "Debe especificar el tipo";
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
                    //msg = " Debe agregar al menos una foto";
                    //return 0;
                    Fotos = new List<ParametrosRegistraFotoMovimientoEmbarqueVBS>();
                }

                //if (Fotos.Count == 0)
                //{
                //    msg = " Debe agregar al menos una foto";
                //    return 0;
                //}

                foreach (ParametrosRegistraFotoMovimientoEmbarqueVBS oParametroFotos in Fotos)
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

        public class ParametrosRegistraFotoMovimientoEmbarqueVBS : Base
        {
            public byte[] foto { get; set; }
            public string ruta { get; set; }
            public string estado { get; set; }

            public int? PreValidationsTransaction(out string msg)
            {
                if (string.IsNullOrEmpty(this.Create_user))
                {
                    msg = "Debe especificar el Create_user responsable";
                    return 0;
                }

                msg = string.Empty;
                return 1;
            }
        }

        public class ParametrosConsultaListaBrandsEmbarque
        {
            public string idExportador { get; set; }

            public int? PreValidationsTransaction(out string msg)
            {
                if ((string.IsNullOrEmpty(idExportador)))
                {
                    msg = "Seleccione el exportador";
                    return 0;
                }

                msg = string.Empty;
                return 1;
            }
        }
    }
}