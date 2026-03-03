using System;
using System.Collections.Generic;
using System.Text;

namespace BRBKApp.Models
{
    public class VHSBoton
    {
        
        public int Id { get; set; }
        public int GrupoEntidadId { get; set; }
        public int Orden { get; set; }
        public string Nombre { get; set; }
        public string Texto { get; set; }
        public string ColorBoton { get; set; }
        public string ColorTexto { get; set; }
        public bool EsVisible { get; set; }
    }
}
