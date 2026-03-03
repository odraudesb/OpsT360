using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiModels.AppModels
{
    public class BAN_Consulta_FilasPorOrden
    {
        public int idFila { get; set; }
        public string nombre { get; set; }
        public int palets { get; set; }
        public int boxes { get; set; }
        public string estado { get; set; }
        public long idOrdenDespacho { get; set; }
        public BAN_Catalogo_Estado oEstado { get; set; }
        public BAN_Stowage_OrdenDespacho oOrdenDespacho { get; set; }
    }
}
