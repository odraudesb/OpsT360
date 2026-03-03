using System;

namespace ApiModels.AppModels
{
    [Serializable]
    public class BAN_Catalogo_Ubicacion : Base
    {
        #region "Propiedades"
        public int? id { get; set; }
        public int? idBodega { get; set; }
        public int? idBloque { get; set; }
        public int? idFila { get; set; }
        public int? idAltura { get; set; }
        public int? idProfundidad { get; set; }
        public string barcode { get; set; }
        public string descripcion { get; set; }
        public int? capacidadBox { get; set; }
        public int? mt2 { get; set; }
        public bool disponible { get; set; }
        public bool? estado { get; set; }
        public string usuarioCrea { get; set; }
        public DateTime? fechaCreacion { get; set; }
        public string usuarioModifica { get; set; }
        public DateTime? fechaModifica { get; set; }
        public BAN_Catalogo_Bodega oBodega { get; set; }
        public BAN_Catalogo_Bloque oBloque { get; set; }
        public BAN_Catalogo_Fila oFila { get; set; }
        public BAN_Catalogo_Altura oAltura { get; set; }
        public BAN_Catalogo_Profundidad oProfundidad { get; set; }
        #endregion
    }
}
