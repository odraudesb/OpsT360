using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MiWebApi
{
    public class ParametrosStowagePlanAisv
    {
        public class ParametrosConsultaListaStowagePlanAisv 
        {
            public string estado { get; set; }
            public string aisv { get; set; }
            public long? idStowageDet { get; set; }

            public int? PreValidationsTransaction(out string msg)
            {
                if (string.IsNullOrEmpty(this.estado) && string.IsNullOrEmpty(this.aisv) && string.IsNullOrEmpty(this.idStowageDet.ToString()))
                {
                    msg = "Especifique un filtro para la consulta";
                    return 0;
                }

                msg = string.Empty;
                return 1;
            }
        }

        public class ParametrosGetStowagPlanAisv 
        {
            public long id { get; set; }

            public int? PreValidationsTransaction(out string msg)
            {
                if (id == 0)
                {
                    msg = "Especifique el Id del StowagPlanAisv";
                    return 0;
                }

                msg = string.Empty;
                return 1;
            }
        }

        public class ParametrosGetStowagPlanAisvXBooking
        {
            public string booking { get; set; }

            public int? PreValidationsTransaction(out string msg)
            {
                if (string.IsNullOrEmpty(booking))
                {
                    msg = "Especifique el booking del StowagPlanAisv";
                    return 0;
                }

                msg = string.Empty;
                return 1;
            }
        }

        public class ParametrosRegistrarAisvExterno : Base
        {
            //public long idStowageCab { get; set; }
            public string idNave { get; set; }
            public int idHold { get; set; }
            public int idBodega { get; set; }
            public int idBloque { get; set; }
            public string aisv { get; set; }

            public int? PreValidationsTransaction(out string msg)
            {
                //if (this.idStowageCab <= 0)
                //{
                //    msg = " Seleccione un stowage plan";
                //    return 0;
                //}
                if (string.IsNullOrEmpty(this.idNave))
                {
                    msg = " Debe especificar nave reference";
                    return 0;
                }
                if (this.idHold <= 0)
                {
                    msg = " Seleccione un hold";
                    return 0;
                }

                if (this.idBodega <= 0)
                {
                    msg = " Seleccione una bodega";
                    return 0;
                }

                if (this.idBloque <= 0)
                {
                    msg = " Seleccione un bloque";
                    return 0;
                }

                if (string.IsNullOrEmpty(this.aisv))
                {
                    msg = " Debe especificar nave reference";
                    return 0;
                }

                if (string.IsNullOrEmpty(this.Create_user))
                {
                    msg = " Debe especificar el Create_user responsable";
                    return 0;
                }

                msg = string.Empty;
                return 1;
            }

        }
    }
}