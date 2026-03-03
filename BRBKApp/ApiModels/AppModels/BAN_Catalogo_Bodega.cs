using System;

namespace ApiModels.AppModels
{
    [Serializable]
    public class BAN_Catalogo_Bodega : Base
    {
        #region "Propiedades"
        public int id { get; set; }
        public string codigo { get; set; }
        public string nombre { get; set; }
        public int idTipo { get; set; }
        public int capacidadBox { get; set; }
        public bool estado { get; set; }
        public string usuarioCrea { get; set; }
        public DateTime? fechaCreacion { get; set; }
        public string usuarioModifica { get; set; }
        public DateTime? fechaModifica { get; set; }
        public BAN_Catalogo_TipoBodega oTipoBodega { get; set; }
        #endregion
    }
}
