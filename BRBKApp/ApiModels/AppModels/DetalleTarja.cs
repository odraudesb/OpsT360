using System;
using System.Collections.Generic;
using System.Text;

namespace BRBKApp.Models
{
    public class DetalleTarja
    {
        public int DetalleTarjaID { get; set; }

        public int TarjaID { get; set; }

        public string TipoCargaDescripcion { get; set; }

        public string InformacionVehiculo { get; set; }

        public string UbicacionBodega { get; set; }

        public DateTime? FechaRetiro { get; set; }

        public string DocumentoTransporte { get; set; }

        public string PackingList { get; set; }

        public string VIN { get; set; }

        public string NumeroMotor { get; set; }

        public string Estado { get; set; }

        public string UsuarioCreacion { get; set; }

        public DateTime FechaCreacionRegistro { get; set; }

        public string UsuarioModificacion { get; set; }

        public DateTime? FechaModificacion { get; set; }
    }
}
