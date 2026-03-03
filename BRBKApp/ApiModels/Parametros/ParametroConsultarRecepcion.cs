using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiModels.Parametros
{
    public class ParametroConsultarRecepcion : Base
    {
        public long Id { get; set; }
        public long idTarjaDet { get; set; }
        public string lugar { get; set; }
    }

    public class ParametroConsultarRecepcionAisv : Base
    {
        public long Id { get; set; }
        public long idStowageAisv { get; set; }
    }

    
    public class ParametroConsultarRecepcionAisvPorBarcode : Base
    {
        public string barcode { get; set; }
    }
    public class ParametroConsultarRecepcionAisvPorId : Base
    {
        public long Id { get; set; }
    }

}
