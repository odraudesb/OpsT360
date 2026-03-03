using System;
using System.Collections.Generic;
using System.Text;

namespace BRBKApp.Models
{
    public class VehiculosDespachados
    {
        public int VehiculoDespachadoID { get; set; }

        public int PasePuertaID { get; set; }

        public int DetalleTarjaID { get; set; }

        public string ObservacionesDespacho { get; set; }

        public string FotosDespacho { get; set; }

        public DateTime FechaDespacho { get; set; }

        public string UsuarioCreacion { get; set; }

        public DateTime FechaCreacionRegistro { get; set; }

        public string UsuarioModificacion { get; set; }

        public DateTime? FechaModificacion { get; set; }
    }
}
