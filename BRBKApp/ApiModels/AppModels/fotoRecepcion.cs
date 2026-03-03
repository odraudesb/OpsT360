using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiModels.AppModels
{
    [Serializable]
    public class fotoRecepcion : Base
    {
        #region "Propiedades"                 
        public long? id { get; set; }
        public long idrecepcion { get; set; }
        public recepcion Recepcion { get; set; }
        public byte[] foto { get;  set; }
        public string ruta { get; set; }
        public string estado { get; set; }
        public estados Estados { get; set; }
        public string usuarioCrea { get; set; }
        public DateTime? fechaCreacion { get; set; }
        public string usuarioModifica { get; set; }
        public DateTime? fechaModifica { get; set; }
        #endregion
    }
}
