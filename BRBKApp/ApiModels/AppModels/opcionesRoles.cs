using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace ApiModels.AppModels
{
    [Serializable]
    public class opcionesRoles : Base
    {
        #region "Propiedades"
        public long Id_Option { get; set; }
        public string Name_Option { get; set; }
        public string Selection { get; set; }
        public int Order { get; set; }
        public long? RoleId { get; set; }
        public string Role_Name { get; set; }

        #endregion
    }
}
