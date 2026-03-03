using System;
using System.Collections.Generic;

namespace ApiModels.AppModels
{
    [Serializable]
    public class BAN_Stowage_Plan_Det : Base
    {
        #region "Propiedades"
        public long? idStowageDet { get; set; }
        public long? idStowageCab { get; set; }
        public int? idHold { get; set; }
        public string piso { get; set; }
        public int? boxSolicitado { get; set; }
        public int? idCargo { get; set; }
        public int? idExportador { get; set; }
        public int? idMarca { get; set; }
        public int? idConsignatario { get; set; }
        public int? idBodega { get; set; }
        public int? idBloque { get; set; }
        public int? boxAutorizado { get; set; }
        public int? reservado { get; set; }
        public int? disponible { get; set; }
        public string comentario { get; set; }
        public int? fechaDocumento { get; set; }
        public string estado { get; set; }
        public string usuarioCrea { get; set; }
        public DateTime? fechaCreacion { get; set; }
        public string usuarioModifica { get; set; }
        public DateTime? fechaModifica { get; set; }
        public string usuarioAnulacion { get; set; }
        public DateTime? fechaAnulacion { get; set; }
        public BAN_Catalogo_Bodega oBodega { get; set; }
        public BAN_Catalogo_Bloque oBloque { get; set; }
        public BAN_Catalogo_Hold oHold { get; set; }
        public BAN_Catalogo_Cargo oCargo { get; set; }
        public BAN_Catalogo_Exportador oExportador { get; set; }
        public BAN_Catalogo_Marca oMarca { get; set; }
        public BAN_Catalogo_Consignatario oConsignatario { get; set; }
        public BAN_Stowage_Plan_Cab oStowage_Plan_Cab { get; set; }
        public List<BAN_Stowage_Plan_Aisv> ListaAISV { get; set; }
        #endregion
    }
}
