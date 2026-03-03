using System;
using System.Collections.Generic;
using System.Text;

namespace BRBKApp.Models
{
    public class RetiroContenedorVacio
    {
        public int RetiroContenedorVacioID { get; set; }

        public int ContenedorID { get; set; }

        public DateTime FechaRetiro { get; set; }

        public string VehiculoRetiro { get; set; }

        public string Estado { get; set; }

        public string UsuarioCreacion { get; set; }

        public DateTime FechaCreacionRegistro { get; set; }

        public string UsuarioModificacion { get; set; }

        public DateTime? FechaModificacion { get; set; }
    }
}
