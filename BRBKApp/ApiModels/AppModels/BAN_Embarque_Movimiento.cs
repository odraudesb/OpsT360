using System;
using System.Collections.Generic;

namespace ApiModels.AppModels
{
    [Serializable]
    public class BAN_Embarque_Movimiento : Base
    {
        #region "Propiedades"
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
        public string usuarioCrea { get; set; }
        public DateTime? fechaCreacion { get; set; }
        public string usuarioModifica { get; set; }
        public DateTime? fechaModifica { get; set; }
        public string usuarioAnulacion { get; set; }
        public DateTime? fechaAnulacion { get; set; }
        public List<fotoEmbarqueVBS> Fotos { get; set; }
        public BAN_Catalogo_TipoMovimiento oTipoMovimiento { get; set; }
        public BAN_Catalogo_Hold oHold { get; set; }
        public BAN_Catalogo_Piso oPiso { get; set; }
        public BAN_Catalogo_Exportador oExportador { get; set; }
        public BAN_Embarque_Cab oEmbarque_Cab { get; set; }
        public BAN_Catalogo_Estado oEstado { get; set; }
        #endregion
    }

    public class fotoEmbarqueVBS : Base
    {
        #region "Propiedades"                 
        public long? id { get; set; }
        public long idMovimiento { get; set; }
        public BAN_Embarque_Movimiento Movimiento { get; set; }
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
