
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiModels.Parametros
{
    public class ParametroConsultarStowagePlanAisv
    {
        public string estado { get; set; }
        public string aisv { get; set; }
        public long? idStowageDet { get; set; }
    }
    public class ParametroConsultarStowagePlanAisvXId
    {
        public long id { get; set; }
    }
}
