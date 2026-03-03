using System;
using System.Collections.Generic;
using System.Text;

namespace BRBKApp.Models
{
    public class OrdenTrabajo
    {
        public int OrdenTrabajoID { get; set; }

        public string NumeroOrden { get; set; }

        public DateTime FechaCreacion { get; set; }

        public string NumeroFactura { get; set; }

        public string BL { get; set; }

        public string Manifiesto { get; set; }

        public int ClienteID { get; set; }

        public string DescripcionProducto { get; set; }

        public string UsuarioCreacion { get; set; }

        public DateTime FechaCreacionRegistro { get; set; }

        public string UsuarioModificacion { get; set; }

        public DateTime? FechaModificacion { get; set; }

        public bool TieneTarja { get; set; }
        public bool TieneContenedor { get; set; }

    }
}
