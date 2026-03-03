using System;

namespace ApiModels.AppModels
{
    [Serializable]
    public class BAN_Embarque_Movimiento_Foto : Base
    {
        #region "Propiedades"                 
        public long id { get; set; }
        public long idMovimiento { get; set; }
        public string ruta { get; set; }
        public string estado { get; set; }
        public string usuarioCrea { get; set; }
        public DateTime? fechaCreacion { get; set; }
        public string usuarioModifica { get; set; }
        public DateTime? fechaModifica { get; set; }
        public BAN_Embarque_Movimiento oEmbarque_Movimiento { get; set; }
        #endregion
    }
}
