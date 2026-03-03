using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace ApiModels.AppModels
{
    [Serializable]
    public class ubicacion : Base
    {
        #region "Propiedades"
        public string id { get; set; }
        public string nombre { get; set; }
        public bool? estado { get; set; }
        #endregion
    }
}

