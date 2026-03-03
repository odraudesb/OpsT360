using System;
using System.Collections.Generic;
using System.Text;

namespace BRBKApp.Models
{
    public class Tarja
    {
        public int TarjaID { get; set; }

        public int OrdenTrabajoID { get; set; }

        public DateTime Fecha { get; set; }

        public string Estado { get; set; }

        public string Observacion { get; set; }

        public string Contenido { get; set; }

        public string UsuarioCreacion { get; set; }

        public DateTime FechaCreacionRegistro { get; set; }

        public string UsuarioModificacion { get; set; }

        public DateTime? FechaModificacion { get; set; }

    }
}
