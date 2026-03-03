using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiModels.AppModels
{
    [Serializable]
    public class BAN_Embarque_Cab: Base
    {
        #region "Propiedades"
        public long? idEmbarqueCab { get; set; }
        public string barcode { get; set; }
        public string idNave { get; set; }
        public string nave { get; set; }
        public string idExportador { get; set; }
        public string Exportador { get; set; }
        public string estado { get; set; }
        public int fechaProceso { get; set; }
        public string usuarioCrea { get; set; }
        public DateTime fechaCreacion { get; set; }
        public string usuarioModifica { get; set; }
        public DateTime? fechaModifica { get; set; }
        public int totalBox { get; set; }
        public int box { get; set; }
        public string detalleBox { get; set; }
        public BAN_Catalogo_Estado oEstado { get; set; }
        public List<BAN_Embarque_Movimiento> oMovimientos { get; set; }
        #endregion
    }
}
