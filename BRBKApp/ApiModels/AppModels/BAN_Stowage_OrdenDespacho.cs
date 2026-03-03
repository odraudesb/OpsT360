using System;
using System.Collections.Generic;

namespace ApiModels.AppModels
{
    [Serializable]
    public class BAN_Stowage_OrdenDespacho  : Base
    {
        #region "Propiedades"
        public long? idOrdenDespacho { get; set; }
        public string idNave { get; set; }
        public int idExportador { get; set; }
        public int idBodega { get; set; }
        public int idBloque { get; set; }
        public int cantidadPalets { get; set; }
        public int cantidadBox { get; set; }
        public int arrastre { get; set; }
        public int pendiente { get; set; }
        public string estado { get; set; }
        public string usuarioCrea { get; set; }
        public DateTime? fechaCreacion { get; set; }
        public string usuarioModifica { get; set; }
        public DateTime? fechaModifica { get; set; }
        public BAN_Catalogo_Estado oEstado { get; set; }
        public BAN_Catalogo_Exportador oExportador { get; set; }
        public BAN_Catalogo_Bloque oBloque { get; set; }
        public BAN_Stowage_Movimiento oMovimiento { get; set; }
        public string booking { get; set; }

        #endregion
    }
}
