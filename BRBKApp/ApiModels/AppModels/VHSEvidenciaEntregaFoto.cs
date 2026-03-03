using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiModels.AppModels
{
    public class VHSEvidenciaEntregaFoto
    {
        public byte[] ArrayFoto { get; set; }
        public string FotoRuta { get; set; }
        public int FotoID { get; set; }
    }

}
