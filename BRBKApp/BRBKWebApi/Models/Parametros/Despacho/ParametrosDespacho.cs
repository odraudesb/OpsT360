using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MiWebApi
{
    public class ParametrosDespacho
    {
        public class ParametrosRegistraDespacho : Base
        {
            public long idTarjaDet { get; set; }

            public string pase { get; set; }
            public string mrn { get; set; }
            public string msn { get; set; }
            public string hsn { get; set; }
            public string placa { get; set; }
            public string idchofer { get; set; }
            public string chofer { get; set; }
            public decimal cantidad { get; set; }
            public string observacion { get; set; }
            public string estado { get; set; }
            public string delivery { get; set; }
            public long PRE_GATE_ID { get; set; }

            public List<ParametrosRegistraFotoDespacho> Fotos { get; set; }

            public int? PreValidationsTransaction(out string msg)
            {
                if (this.idTarjaDet <= 0)
                {
                    msg = "Seleccione una recepción";
                    return 0;
                }

                if (string.IsNullOrEmpty(this.pase))
                {
                    msg = "Debe ingresar el número de pase";
                    return 0;
                }

                if (string.IsNullOrEmpty(this.mrn))
                {
                    msg = "Debe ingresar el MRN";
                    return 0;
                }

                if (string.IsNullOrEmpty(this.msn))
                {
                    msg = "Debe ingresar el MSN";
                    return 0;
                }

                if (string.IsNullOrEmpty(this.hsn))
                {
                    msg = "Debe ingresar el HSN";
                    return 0;
                }

                if (string.IsNullOrEmpty(this.placa))
                {
                    msg = "Debe ingresar la placa";
                    return 0;
                }

                if (string.IsNullOrEmpty(this.idchofer))
                {
                    msg = "Debe ingresar el chofer";
                    return 0;
                }

                if (this.cantidad == 0)
                {
                    msg = "Debe ingresar la cantidad";
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

                foreach (ParametrosRegistraFotoDespacho oParametroFotos in Fotos)
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

        public class ParametrosRegistraFotoDespacho : Base
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

        public class ParametrosConsultaPasePuerta : Base
        {
            public string numeroPase { get; set; }

            public int? PreValidationsTransaction(out string msg)
            {
                if (string.IsNullOrEmpty(this.numeroPase))
                {
                    msg = "Seleccione un pase";
                    return 0;
                }

                msg = string.Empty;
                return 1;
            }
        }


        public class ParametrosLoadTruckN4 : Base
        {
            public string pase { get; set; }
            public string bl { get; set; }
            public string placa { get; set; }
            public decimal cantidad { get; set; }

            public int? PreValidationsTransaction(out string msg)
            {
            
                if (string.IsNullOrEmpty(this.pase))
                {
                    msg = "Debe ingresar el número de pase";
                    return 0;
                }

                if (string.IsNullOrEmpty(this.bl))
                {
                    msg = "Debe ingresar el BL";
                    return 0;
                }

                if (string.IsNullOrEmpty(this.placa))
                {
                    msg = "Debe ingresar la placa";
                    return 0;
                }

                
                if (this.cantidad == 0)
                {
                    msg = "Debe ingresar la cantidad";
                    return 0;
                }

                if (string.IsNullOrEmpty(this.Create_user))
                {
                    msg = "Debe especificar el Create_user responsable";
                    return 0;
                }

                string oError = string.Empty;

                
                if (!string.IsNullOrEmpty(oError))
                {
                    msg = oError;
                    return 0;
                }

                msg = string.Empty;
                return 1;
            }
        }
    }
}