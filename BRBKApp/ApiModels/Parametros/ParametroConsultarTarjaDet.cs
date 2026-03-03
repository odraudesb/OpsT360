using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiModels.Parametros
{
    public class ParametroConsultarTarjaDet : Base
    {
        public  string MRN { get; set; }
        public string Lugar { get; set; }
    }
    public class ParametroConsultarTarjaDetXId : Base
    {
        public long IdTarjaDet { get; set; }
    }
}
