using System;
using System.Collections.Generic;

namespace ApiModels.AppModels
{
    [Serializable]
    public class BAN_Catalogo_Piso : Base
    {
        #region "Propiedades"
        public int? id { get; set; }
        public string nombre { get; set; }
        public bool? estado { get; set; }
        public string usuarioCrea { get; set; }
        public DateTime? fechaCreacion { get; set; }
        public string usuarioModifica { get; set; }
        public DateTime? fechaModifica { get; set; }
        #endregion
    }
}
