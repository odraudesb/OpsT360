using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiModels.AppModels
{
    public class VHSMensajeSimple
    {
        public long VehiculoDespachadoID { get; set; }
        public int DetalleTarjaID { get; set; }   // NUEVO CAMPO
        public string Mensaje { get; set; }
    }


}
