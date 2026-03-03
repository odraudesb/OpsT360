using System;
using System.Collections.Generic;

namespace MiWebApi
{
    public class ParametrosRecepcionVBS
    {
        public class ParametrosConsultaListaRecepcionAisv
        {
            public long idStowageAisv { get; set; }

            public int? PreValidationsTransaction(out string msg)
            {
                if (this.idStowageAisv <= 0)
                {
                    msg = "Seleccione un AISV";
                    return 0;
                }

                msg = string.Empty;
                return 1;
            }
        }

        public class ParametrosConsultaRecepcionAisv 
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

        public class ParametrosConsultaRecepcionAisvPorBarcode
        {
            public string barcode { get; set; }

            public int? PreValidationsTransaction(out string msg)
            {
                if (string.IsNullOrEmpty(barcode))
                {
                    msg = "Especifique el barcode de la recepción";
                    return 0;
                }

                msg = string.Empty;
                return 1;
            }
        }

        public class ParametrosRegistraRecepcionAsv : Base
        {
            public long idStowageAisv { get; set; }

            public int idModalidad { get; set; }
            public string tipo { get; set; }
            public int cantidad { get; set; }
            public string observacion { get; set; }
            public string estado { get; set; }

            public List<ParametrosRegistraFotoRecepcionAisv> Fotos { get; set; }

            public int? PreValidationsTransaction(out string msg)
            {
                if (this.idStowageAisv <= 0)
                {
                    msg = "Seleccione un AISV";
                    return 0;
                }
                if (this.idModalidad <= 0)
                {
                    msg = "Seleccione una Modalidad";
                    return 0;
                }

                if (cantidad == 0)
                {
                    msg = "Debe especificar la cantidad";
                    return 0;
                }

                if (string.IsNullOrEmpty(this.tipo))
                {
                   msg = "Debe especificar el tipo [ING]/[EGR]";
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

                if (Fotos.Count ==0)
                {
                    msg = "Debe agregar al menos una foto";
                    return 0;
                }

                foreach (ParametrosRegistraFotoRecepcionAisv oParametroFotos in Fotos)
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

        public class ParametrosActualizaRecepcionAisv : Base
        {
            public Int64? idMovimiento { get; set; }
            public long idStowageAisv { get; set; }
            public int cantidad { get; set; }
            public int idModalidad { get; set; }
            public int idUbicacion { get; set; }
            public string tipo { get; set; }
            public string observacion { get; set; }
            public string estado { get; set; }
            public bool isMix { get; set; }
            public string referencia { get; set; }
            public int? PreValidationsTransaction(out string msg)
            {
                if (this.idMovimiento <= 0)
                {
                    msg = "Seleccione una recepción";
                    return 0;
                }

                if (this.idStowageAisv <= 0)
                {
                    msg = "Seleccione un AISV";
                    return 0;
                }

                if (this.idUbicacion <= 0)
                {
                    msg = "Seleccione una ubicación";
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

        public class ParametrosRegistraFotoRecepcionAisv : Base
        {
            public byte[] foto { get; set; }
            public string ruta { get; set; }
            public string estado { get; set; }

            public int? PreValidationsTransaction(out string msg)
            {
                //if (string.IsNullOrEmpty(this.estado))
                //{
                //    msg = "Debe especificar el estado";
                //    return 0;
                //}

                if (string.IsNullOrEmpty(this.Create_user))
                {
                    msg = "Debe especificar el Create_user responsable";
                    return 0;
                }

                msg = string.Empty;
                return 1;
            }
        }

        public class ParametrosAnularRecepcionAisv : Base
        {
            public Int64? idMovimiento { get; set; }
           
            public int? PreValidationsTransaction(out string msg)
            {
                if (this.idMovimiento <= 0)
                {
                    msg = " Seleccione una recepción";
                    return 0;
                }

                if (string.IsNullOrEmpty(this.Modifie_user))
                {
                    msg = " Debe especificar el Modifie_user responsable";
                    return 0;
                }


                msg = string.Empty;
                return 1;
            }

        }

        //Pre Despacho

        public class ParametrosConsultaListaRecepcionPorNoOrden
        {
            public long idOrdenDespacho { get; set; }

            public int? PreValidationsTransaction(out string msg)
            {
                if (this.idOrdenDespacho <= 0)
                {
                    msg = "Seleccione la orden de despacho";
                    return 0;
                }

                msg = string.Empty;
                return 1;
            }
        }

        //despacho

        public class ParametrosRegistrarDespachoAisv : Base
        {
            public Int64? idMovimiento { get; set; }
            public string tipo { get; set; }
            
            public List<ParametrosRegistraFotoRecepcionAisv> Fotos { get; set; }

            public int? PreValidationsTransaction(out string msg)
            {
                if (this.idMovimiento <= 0)
                {
                    msg = " Seleccione una recepción";
                    return 0;
                }
              
                if (string.IsNullOrEmpty(this.Create_user))
                {
                    msg = " Debe especificar el Create_user responsable";
                    return 0;
                }

                string oError = string.Empty;

                if (Fotos == null)
                {
                    msg = " Debe agregar al menos una foto";
                    return 0;
                }

                if (Fotos.Count == 0)
                {
                    msg = " Debe agregar al menos una foto";
                    return 0;
                }

                foreach (ParametrosRegistraFotoRecepcionAisv oParametroFotos in Fotos)
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

    }
}


