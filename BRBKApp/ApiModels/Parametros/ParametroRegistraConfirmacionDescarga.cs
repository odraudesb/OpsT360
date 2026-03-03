using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiModels.Parametros
{
    public class ParametroRegistraConfirmacionDescarga : Base
    {
        public long? gkey { get; set; }
        public string container { get; set; }
        public string dataContainer { get; set; }
        public string position { get; set; }
        public string referencia { get; set; }
    }
    public class ParametroObtenerDataContainersImpo : Base
    {
        public string numcontainers { get; set; }
    }
}
