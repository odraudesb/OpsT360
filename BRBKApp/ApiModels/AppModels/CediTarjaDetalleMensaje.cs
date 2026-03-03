
using System;

using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiModels.AppModels
{
    public class CediTarjaDetalleMensaje
    {
        public int DetalleTarjaID { get; set; }
        public int TarjaID { get; set; }
        public string TipoCargaDescripcion { get; set; }
        public string InformacionVehiculo { get; set; }
        public string UbicacionBodega { get; set; }
        public string DocumentoTransporte { get; set; }
        public string PackingList { get; set; }
        public string VIN { get; set; }
        public string NumeroMotor { get; set; }
        public string Observaciones { get; set; }
        public List<CediTarjaDetalleFoto> Fotos { get; set; }
        public string Mensaje { get; set; }

        public CediBloque Bloque { get; set; } // Cambiado de CediBloque a int?

        public int NumeroBloque { get; set; }
        public int Id { get; set; }
        public string Nombre_Bloque { get; set; }
        public int Capacidad { get; set; }
    }

}
