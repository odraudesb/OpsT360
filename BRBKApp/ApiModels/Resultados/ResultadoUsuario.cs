using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiModels
{
    [Serializable]
    public class ResultadoUsuario : Base
    {
        #region "Propiedades"     

        public Int64 Id { get; set; }
        public string Names { get; set; }
        public string Username { get; set; }
        public string Identification { get; set; }
        public Int64 PositionId { get; set; }
        public Int64? RoleId { get; set; }
        public string Password { get; set; }
        public string Phone { get; set; }
        public Int64? CompanyId { get; set; }
        public Int64? Timer { get; set; }
        #endregion

    }
}
