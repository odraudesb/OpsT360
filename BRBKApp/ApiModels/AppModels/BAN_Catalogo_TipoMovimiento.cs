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
    public class BAN_Catalogo_TipoMovimiento : Base
    {
        #region "Propiedades"
        public int idTipo { get; set; }
        public string descripcion { get; set; }
        public bool? estado { get; set; }
        #endregion
    }
}
