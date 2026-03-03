using System;

namespace ApiModels.AppModels
{
    [Serializable]
    public class BAN_HorarioFinal : Base
    {
        #region "Propiedades"
        public string Id_Hora { get; set; }
        public int Id_HorarioIni { get; set; }
        public string codigo { get; set; }
        public string Desc_Hora { get; set; }
        public bool estado { get; set; }
        #endregion
    }
}
