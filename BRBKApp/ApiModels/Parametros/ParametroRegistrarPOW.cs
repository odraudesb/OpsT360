using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiModels.Parametros
{
    public class ParametroRegistrarPOW : Base
    {
        public long? id { get; set; }
        public string ip { get; set; }
        public string imei { get; set; }
        public string idPosition { get; set; }
        public string namePosition { get; set; }
        public bool estado { get; set; }
        public DateTime validoHasta { get; set; }
        public string usuarioCrea { get; set; }
    }

    public class ParametroGetListPOWN4 : Base
    {
        public string parametro1 { get; set; }
    }

    public class ParametroObtenerPOW : Base
    {
        public string idPosition { get; set; }
        public string usuarioCrea { get; set; }
        public string containers { get; set; }
    }
}
