using System;
using System.Collections.Generic;

namespace ApiModels.AppModels
{
    [Serializable]
    public class BAN_Stowage_Movimiento : Base
    {
        #region "Propiedades"
        public long? idMovimiento { get; set; }
        public long idStowageAisv { get; set; }
        public int? idUbicacion { get; set; }
        public int? fechaProceso { get; set; }
        public int? anio { get; set; }
        public int? mes { get; set; }
        public int? dia { get; set; }
        public string barcode { get; set; }
        public int idModalidad { get; set; }
        public string tipo { get; set; }
        public int cantidad { get; set; }
        public string observacion  { get; set; }
        public string estado { get; set; }
        public bool active { get; set; }
        public long idOrdenDespacho { get; set; }
        public bool isOrdenActive { get; set; }
        public string usuarioCrea { get; set; }
        public DateTime? fechaCreacion { get; set; }
        public string usuarioModifica { get; set; }
        public DateTime? fechaModifica { get; set; }
        public BAN_Catalogo_Ubicacion oUbicacion { get; set; }
        public BAN_Stowage_Plan_Aisv oStowage_Plan_Aisv { get; set; }
        public BAN_Catalogo_Estado oEstado { get; set; }
        public BAN_Catalogo_Modalidad oModalidad { get; set; }

        public List<fotoRecepcionVBS> Fotos { get; set; }

        public BAN_Catalogo_Bloque oBloque { get; set; }
        public BAN_Catalogo_Exportador oExportador { get; set; }
        public int palets { get; set; }
        public int idExportador { get; set; }
        public string idNave { get; set; }
        public string booking { get; set; }

        public bool isMix { get; set; }
        public string referencia { get; set; }
        #endregion
    }

    [Serializable]
    public class fotoRecepcionVBS : Base
    {
        #region "Propiedades"                 
        public long? id { get; set; }
        public long idMovimiento { get; set; }
        public BAN_Stowage_Movimiento Movimiento { get; set; }
        public byte[] foto { get; set; }
        public string ruta { get; set; }
        public string estado { get; set; }
        public string usuarioCrea { get; set; }
        public DateTime? fechaCreacion { get; set; }
        public string usuarioModifica { get; set; }
        public DateTime? fechaModifica { get; set; }
        #endregion
    }
}

