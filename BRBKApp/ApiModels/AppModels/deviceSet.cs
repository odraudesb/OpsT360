using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiModels.AppModels
{
    public class deviceSet
    {
        public long? Id { get; set; }
        public string Names { get; set; }
        public string Create_user { get; set; }
        public string Modifie_user { get; set; }
        public DateTime? Create_date { get; set; }
        public DateTime? Modifie_date { get; set; }
        public bool? Status { get; set; }
        public string Imei { get; set; }

    }
}
