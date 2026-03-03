using System;
using System.Collections.Generic;

namespace MiWebApi
{
    public class ParametrosDespachoVBS
    {
        public class ParametrosConsultaCargaEnBodegaAisv
        {
            public string idNave { get; set; }
            public int? idBodega { get; set; }
            public int? idBloque { get; set; }
            public int? idExportador { get; set; }
            public string booking { get; set; }
            public string barcode { get; set; }

            public int? PreValidationsTransaction(out string msg)
            {
                if (string.IsNullOrEmpty(idNave) && idBodega is null && idExportador is null && string.IsNullOrEmpty(booking) && string.IsNullOrEmpty(barcode))
                {
                    msg = "Seleccione al menos un filtro para la consulta";
                    return 0;
                }

                msg = string.Empty;
                return 1;
            }
        }

        public class ParametrosConsultaListaOrdenesDespacho
        {
            public string idNave { get; set; }
            public int? idExportador { get; set; }
            public int? idBloque { get; set; }
            public string  booking { get; set; }

            public int? PreValidationsTransaction(out string msg)
            {
                if (string.IsNullOrEmpty(idNave))
                {
                    msg = "Seleccione la nave";
                    return 0;
                }

                if (string.IsNullOrEmpty(booking))
                {
                    msg = "Seleccione la nave";
                    return 0;
                }

                if (idExportador is null)
                {
                    msg = "Seleccione el exportador";
                    return 0;
                }

                if (idBloque is null)
                {
                    msg = "Seleccione el bloque";
                    return 0;
                }

                msg = string.Empty;
                return 1;
            }
        }
        public class ParametrosConsultaListaOrdenesDespachoPorBodega
        {
            public int? idBodega { get; set; }

            public int? PreValidationsTransaction(out string msg)
            {
                if (idBodega is null)
                {
                    msg = "Seleccione la bodega";
                    return 0;
                }

                msg = string.Empty;
                return 1;
            }
        }
        public class ParametrosConsultaListaOrdenesDespachoAgrupadas
        {
            public long idOrdenDespacho { get; set; }

            public int? PreValidationsTransaction(out string msg)
            {
                if (idOrdenDespacho <= 0)
                {
                    msg = "Seleccione la la orden de despacho";
                    return 0;
                }

                msg = string.Empty;
                return 1;
            }
        }

        public class ParametrosConsultaListaOrdenesDespachoFila
        {
            public long idOrdenDespacho { get; set; }
            public int idFila { get; set; }

            public int? PreValidationsTransaction(out string msg)
            {
                if (idOrdenDespacho <= 0)
                {
                    msg = "Seleccione la orden de despacho";
                    return 0;
                }

                if (idFila <= 0)
                {
                    msg = "Seleccione la fila";
                    return 0;
                }

                msg = string.Empty;
                return 1;
            }
        }

        public class ParametrosRegistrarOrdenDespacho: Base
        {
            public long idOrdenDespacho { get; set; }
            public string idNave { get; set; }
            public int idExportador { get; set; }
            public int idBodega { get; set; }
            public int idBloque { get; set; }
            public int cantidadPalets { get; set; }
            public int cantidadBox { get; set; }
            public int arrastre { get; set; }
            public int pendiente { get; set; }
            public string estado { get; set; }
            public string booking { get; set; }

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

                if (string.IsNullOrEmpty(this.idNave))
                {
                    msg = "Debe especificar la nave";
                    return 0;
                }

                if (this.idExportador <= 0)
                {
                    msg = "Seleccione el exportador";
                    return 0;
                }
                if (this.idBodega <= 0)
                {
                    msg = "Seleccione la bodega";
                    return 0;
                }

                if (this.idBloque == 0)
                {
                    msg = "Debe especificar el bloque";
                    return 0;
                }

                if (this.cantidadPalets == 0)
                {
                    msg = "Debe especificar la cantidad Palets";
                    return 0;
                }
                if (this.cantidadBox == 0)
                {
                    msg = "Debe especificar la cantidad Box";
                    return 0;
                }

                if (string.IsNullOrEmpty(this.booking))
                {
                    msg = "Debe especificar el booking";
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

        public class ParametrosRegistrarPreDespacho : Base
        {
            public long idMovimiento { get; set; }

            public int? PreValidationsTransaction(out string msg)
            {
                if (this.idMovimiento == 0)
                {
                    msg = "Debe especificar el idMovimiento";
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