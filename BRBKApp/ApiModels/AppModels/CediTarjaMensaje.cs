
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiModels.AppModels
{
    public class CediTarjaMensaje
    {
        public int TarjaId { get; set; }
        public int OrdenTrabajoID { get; set; }
        public string Mensaje { get; set; }
        public List<CediTarjaDetalleMensaje> Detalle { get; set; }
        public int DetalleTarjaID { get; set; }


        public string DescripcionProducto { get; set; }

        public string Origen { get; set; }

        public long VehiculoDespachadoID { get; set; }



    }
}
