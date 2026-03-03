using System;
using System.Collections.Generic;
using System.Text;

namespace BRBKApp.Models
{
    public class Contenedor
    {
        public int ContenedorID { get; set; }

        public string IdentificadorUnico { get; set; }

        public string NumeroContenedor { get; set; }

        public string NombreNave { get; set; }

        public int ClienteID { get; set; }

        public int OrdenTrabajoID { get; set; }

        public string Vehiculo { get; set; }

        public string Estado { get; set; }

        public string UsuarioCreacion { get; set; }

        public DateTime FechaCreacionRegistro { get; set; }

        public string UsuarioModificacion { get; set; }

        public DateTime? FechaModificacion { get; set; }

    }
}
