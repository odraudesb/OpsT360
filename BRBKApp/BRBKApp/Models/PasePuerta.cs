using System;
using System.Collections.Generic;
using System.Text;

namespace BRBKApp.Models
{
    public class PasePuerta
    {
        public int PasePuertaID { get; set; }

        public string NumeroPase { get; set; }

        public DateTime FechaCreacion { get; set; }

        public DateTime? FechaRetiro { get; set; }

        public string PlacaCamion { get; set; }

        public string ChoferID { get; set; }

        public string EmpresaTransporteID { get; set; }

        public string Estado { get; set; }

        public string UsuarioCreacion { get; set; }

        public DateTime FechaCreacionRegistro { get; set; }

        public string UsuarioModificacion { get; set; }

        public DateTime? FechaModificacion { get; set; }
    }
}
