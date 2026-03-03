using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
namespace ViewModel
{
    [Serializable]
    public class RespuestaViewModel<T>
    {
        public T Respuesta { get; set; }
        public ResultadoViewModel Resultado { get; set; }
    }
}
