using System;
using System.Collections.Generic;
using System.Text;

namespace BRBKApp.Models
{
    public class EvidenciaEntrega
    {
        public int EvidenciaEntregaID { get; set; }

        public int VehiculoDespachadoID { get; set; }

        public string FotoEntrega { get; set; }

        public string UsuarioCreacion { get; set; }

        public DateTime FechaCreacionRegistro { get; set; }

        public string UsuarioModificacion { get; set; }

        public DateTime? FechaModificacion { get; set; }

    }
}
