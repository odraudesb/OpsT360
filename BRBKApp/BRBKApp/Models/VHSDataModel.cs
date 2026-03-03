using System;
using System.Collections.Generic;
using System.Text;

namespace BRBKApp.Models
{
    public class VHSDataModel<T> where T : class
    {
        public int Id { get; set; }
        public List<VHSCampo> ValoresEntidad { get; set; }
        public List<VHSBoton> Botones { get; set; }
        public T Entidad { get; set; }
    }
}
