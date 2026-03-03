using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace ApiModels.AppModels
{
    [Serializable]
    public class carrier : Base
    {
        #region "Propiedades"
        public string carrier_id { get; set; }
        public string ruc { get; set; }
        public string nombre { get; set; }
        #endregion
    }
}
