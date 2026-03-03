using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiModels.AppModels
{
    public class ValidaPaletizado : Base
    {

        public long? gkey { get; set; }
        public string contenedor { get; set; }
        public int servicio { get; set; }


    }
}
