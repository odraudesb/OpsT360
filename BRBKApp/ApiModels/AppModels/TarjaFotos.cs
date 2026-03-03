using System;
using System.Collections.Generic;
using System.Text;

namespace BRBKApp.Models
{
    public class TarjaFotos
    {
        public int TarjaFotoID { get; set; }

        public int TarjaID { get; set; }

        public string FotoContenedor { get; set; }

        public string UsuarioCreacion { get; set; }

        public DateTime FechaCreacionRegistro { get; set; }

        public string UsuarioModificacion { get; set; }

        public DateTime? FechaModificacion { get; set; }

    }
}
