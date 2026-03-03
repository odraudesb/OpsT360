using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiModels.AppModels
{
    [Serializable]
    public class User : Base
    {
        [PrimaryKey]
        public long UserId { get; set; }
        public long Id { get; set; }
        public string Names { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Identification { get; set; }
        public string Phone { get; set; }
        public string Create_user { get; set; }
        public string Modifie_user { get; set; }
        public System.DateTime Create_date { get; set; }
        public Nullable<System.DateTime> Modifie_date { get; set; }
        public bool Status { get; set; }
        public long PositionId { get; set; }
        public Nullable<long> RoleId { get; set; }
        public Nullable<long> CompanyId { get; set; }
        public Nullable<long> IdUserLeader { get; set; }
        public string Email { get; set; }
        public Nullable<System.DateTime> WorkDate { get; set; }

        public Position Position { get; set; }
        public Role Role { get; set; }
        public int Timer { get; set; }
        public UserDB UsuarioDB { get; set; }
    }

    public class UserDB : Base
    {
        [PrimaryKey]
        public long UserId { get; set; }
        public long Id { get; set; }

        public string Names { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Identification { get; set; }
        public string Phone { get; set; }
        public string Create_user { get; set; }
        public string Modifie_user { get; set; }
        public System.DateTime Create_date { get; set; }
        public Nullable<System.DateTime> Modifie_date { get; set; }
        public bool Status { get; set; }
        public long PositionId { get; set; }
        public Nullable<long> RoleId { get; set; }
        public Nullable<long> CompanyId { get; set; }
        public Nullable<long> IdUserLeader { get; set; }
        public string Email { get; set; }
        public Nullable<System.DateTime> WorkDate { get; set; }

        public int Timer { get; set; }

    }
}
