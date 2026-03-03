using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace ApiModels.AppModels
{
    [Serializable]
    public class parametros 
    {
        #region "Propiedades"
        public string aplicacion { get; set; }
        public string modulo { get; set; }
        public string nombre { get; set; }
        public string valor { get; set; }
        public string tipodato { get; set; }
        #endregion
    }
}
