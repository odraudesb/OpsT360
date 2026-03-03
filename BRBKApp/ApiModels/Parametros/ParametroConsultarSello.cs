using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiModels.Parametros
{
    public class ParametroConsultarSello: Base
    {
        public string container { get; set; }
        public string seal { get; set; }
        public string position { get; set; }
        public long? gkey { get; set; }
        public bool bloqueo { get; set; }
        public bool impedimento { get; set; }
        public string impedimentod { get; set; }
        public List<ParametroRegistrarSelloFoto> Fotos { get; set; }
    }

    public class ParametroRegistrarSelloFoto : Base
    {
        public byte[] foto { get; set; }
        public string ruta { get; set; }
        public string estado { get; set; }
    }
}
