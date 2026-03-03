using System;

namespace ApiModels.AppModels
{
    [Serializable]
    public class BAN_Catalogo_Profundidad : Base
    {
        #region "Propiedades"
        public int id { get; set; }
        public string descripcion { get; set; }
        public bool estado { get; set; }
        #endregion
    }
}
