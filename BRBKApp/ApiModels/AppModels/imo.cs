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
    public class imos : Base
    {
        #region "Propiedades"
        public string id { get; set; }
        public string imo { get; set; }
        public bool? estado { get; set; }
        #endregion
    }
}
