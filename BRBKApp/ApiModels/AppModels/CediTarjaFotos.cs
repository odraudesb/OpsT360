
using System;


using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiModels.AppModels
{
    public class CediTarjaFotos
    {
        public int TarjaFotoId { get; set; }
        public int TarjaId { get; set; }
        public int Orden { get; set; }
        public string Ruta { get; set; }
        public byte[] ArrayFoto { get; set; }
    }
}
