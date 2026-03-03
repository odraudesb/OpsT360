using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace MiWebApi
{
    [Serializable]
    public class ResultadoConsultaUsuario : Base
    {
        #region "Propiedades"     

 
        public Int64 Id { get; set; }
        public string Names { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Identification { get; set; }
        public string Phone { get; set; }
        public Int64 PositionId { get; set; }
        public string Position_name { get; set; }
        public Int64? RoleId { get; set; }
        public string RoleName { get; set; }
        public Int64? CompanyId { get; set; }
        public string Company_name { get; set; }
        public Int64? IdUserLeader { get; set; }
        public string Email { get; set; }
        public string UserLeader_name { get; set; }
        public DateTime? WorkDate { get; set; }
        #endregion

    }
}