using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiModels.Parametros
{
    public class ParametroRegistrarContainersPesaje : Base
    {
        public long gkey { get; set; }
        public string container { get; set; }
        public string peso { get; set; }
        public bool estado { get; set; }
        public string ip { get; set; }
        public string mensaje { get; set; }
    }
}
