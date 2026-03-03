using System;

namespace ApiModels.AppModels
{
    [Serializable]
    public class BAN_HorarioInicial : Base
    {
        #region "Propiedades"
        public int Id_Hora { get; set; }
        public string Desc_Hora { get; set; }
        public bool estado { get; set; }
        #endregion
    }
}
