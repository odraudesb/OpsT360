using System;

namespace ApiModels.AppModels
{
    [Serializable]
    public class BAN_Catalogo_Exportador : Base
    {
        #region "Propiedades"
        public int? id { get; set; }
        public int? idLinea { get; set; }
        public string ruc { get; set; }
        public string nombre { get; set; }
        public bool? estado { get; set; }
        public string usuarioCrea { get; set; }
        public DateTime? fechaCreacion { get; set; }
        public string usuarioModifica { get; set; }
        public DateTime? fechaModifica { get; set; }
        public BAN_Catalogo_Linea Linea { get; set; }
        #endregion
    }
}
