using System;

namespace ApiModels.AppModels
{
    public class BAN_Catalogo_Cargo : Base
    {
        #region "Propiedades"
        public int id { get; set; }
        public string nombre { get; set; }
        public bool estado { get; set; }
        public string usuarioCrea { get; set; }
        public DateTime fechaCreacion { get; set; }
        public string usuarioModifica { get; set; }
        public DateTime? fechaModifica { get; set; }
        #endregion
    }
}
