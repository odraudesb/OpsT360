using System;

namespace ApiModels.AppModels
{
    [Serializable]
    public class BAN_Stowage_Plan_Cab : Base
    {
        #region "Propiedades"
        public long idStowageCab { get; set; }
        public string idNave { get; set; }
        public string nave { get; set; }
        public int idLinea { get; set; }
        public string linea { get; set; }
        public bool estado { get; set; }
        public int fechaDocumento { get; set; }
        public string usuarioCrea { get; set; }
        public DateTime fechaCreacion { get; set; }
        public string usuarioModifica { get; set; }
        public DateTime? fechaModifica { get; set; }
        #endregion
    }
}
