using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace N4Ws.Entidad
{
    internal class N4Configuration
    {
        public string url { get; private set; }
        public string usuario { get; private set; }
        public string password { get; private set; }
        public string scope { get; private set; }

        private N4Configuration ( string _u, string _us, string _pa, string _sc )
        {
            this.url = _u?.Trim().ToLower();
            this.usuario = _us?.Trim().ToLower();
            this.password = _pa;
            this.scope = _sc?.Trim().ToLower();
        }

        private static N4Configuration instance = null;

        public static N4Configuration GetInstance(string us, string pass, string url, string sco )
        {
            if (instance == null)
                instance = new N4Configuration(url, us, pass,  sco);
            return instance;
        }
    }
}
