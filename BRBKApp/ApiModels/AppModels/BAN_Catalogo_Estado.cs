using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiModels.AppModels
{
    [Serializable]
    public class BAN_Catalogo_Estado : Base
    {
        #region "Propiedades"
        public string id { get; set; }
        public string nombre { get; set; }
        public bool? estado { get; set; }
        #endregion
    }
}
