using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiModels.AppModels
{
    [Serializable]
    public class tarjaCab : Base
    {
        #region "Propiedades"
        public long? idTarja { get; set; }
        public string idNave { get; set; }
        public string nave { get; set; }
        public string carrier_id { get; set; }
        public string idAgente { get; set; }
        public string Agente { get; set; }
        public string mrn { get; set; }
        public DateTime eta { get; set; }
        public DateTime? fecha { get; set; }
        public string estado { get; set; }
        public estados Estados { get; set; }
        public string usuarioCrea { get; set; }
        public DateTime fechaCreacion { get; set; }
        public string usuarioModifica { get; set; }
        public DateTime? fechaModifica { get; set; }
        public List<tarjaDet> Detalle { get; set; }
        #endregion
    }
}
