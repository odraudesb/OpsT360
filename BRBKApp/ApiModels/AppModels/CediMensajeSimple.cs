
using System;


using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiModels.AppModels
{
    public class CediMensajeSimple
    {
        public long VehiculoDespachadoID { get; set; }
        public int DetalleTarjaID { get; set; }
        public string Mensaje { get; set; }
    }
}
