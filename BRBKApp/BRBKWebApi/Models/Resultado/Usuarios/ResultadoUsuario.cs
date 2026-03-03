using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using ApiModels.AppModels;

namespace MiWebApi
{
    [Serializable]
    public class ResultadoUsuario:Base
    {
        #region "Propiedades"     

        //[JsonProperty("codigo")]
        public Int64 Id { get; set; }
        //[JsonProperty("nombres")]
        public string Names { get; set; }
        //[JsonProperty("usuario")]
        public string Username { get; set; }
        //[JsonProperty("cedula")]
        public string Identification { get; set; }
        //[JsonProperty("id del cargo")]
        public Int64 PositionId { get; set; }
        //[JsonProperty("id del rol asignado")]
        public Int64? RoleId { get; set; }

        public string Password { get; set; }
        public string Phone { get; set; }

        public Int64? CompanyId { get; set; }

        public Int64? Timer { get; set; }

        public Int64? IdUserLeader { get; set; }

        public string Email { get; set; }

        public DateTime? WorkDate { get; set; }
        public Role Role { get; set; }
        public Position Position { get; set; }

        #endregion
    }
}