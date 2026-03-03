using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiModels.AppModels
{
    [Serializable]
    public class workPosition : Base
    {
        #region "Propiedades"
        public long? id { get; set; }
        public string ip { get; set; }
        public string imei { get; set; }
        public string idPosition { get; set; }
        public string namePosition { get; set; }
        public bool estado { get; set; }
        public DateTime validoHasta { get; set; }
        public string usuarioCrea { get; set; }
        public dataContainers dataContenedor { get; set; }
        #endregion
    }

    [Serializable]
    public class workPositionN4List : Base
    {
        #region "Propiedades"
        public string codigo { get; set; }
        public string nave { get; set; }
        #endregion
    }

    [Serializable]
    public class dataContainers : Base
    {
        public long gkey { get; set; }
        public string puerto1 { get; set; }
        public string puerto2 { get; set; }
        public string tipo { get; set; }
        public double peso { get; set; }
        public string impedimentod { get; set; }
        public bool impedimento { get; set; }
        public double temperatura { get; set; }
        public int imo { get; set; }
        public string sello { get; set; }
        public bool bloqueo { get; set; }
        public string visit { get; set; }
        public string notas { get; set; }
        public string cntr { get; set; }
    }
}
