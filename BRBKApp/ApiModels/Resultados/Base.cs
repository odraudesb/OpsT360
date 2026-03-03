using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiModels
{
    public abstract class Base
    {
        protected static Dictionary<string, object> parametros = null;
        public string Create_user { get; set; }
        public string Modifie_user { get; set; }

        public DateTime? Create_date { get; set; }
        public DateTime? Modifie_date { get; set; }
        public bool? Status { get; set; }

        protected virtual void init()
        {
            parametros = new Dictionary<string, object>();
        }

    }
}
