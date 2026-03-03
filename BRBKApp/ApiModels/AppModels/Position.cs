using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiModels.AppModels
{
    [Serializable]
    public class Position: Base
    {
        public long Id { get; set; }
        public string Description { get; set; }
        public string Create_user { get; set; }
        public string Modifie_user { get; set; }
        public System.DateTime Create_date { get; set; }
        public Nullable<System.DateTime> Modifie_date { get; set; }
        public bool Status { get; set; }
    }
}
