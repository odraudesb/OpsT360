using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
namespace ViewModel
{
    [Serializable]
    public class ResultadoViewModel
    {
        public List<string> Mensajes { get; set; }
        public bool Respuesta { get; set; }
        public Enumerados.TipoMensaje TipoMensaje { get; set; }
        public string Titulo { get; set; }
        public long TotalRowsCount { get; set; }
    }
}
