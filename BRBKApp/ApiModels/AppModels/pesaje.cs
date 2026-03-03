using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiModels.AppModels
{
    [Serializable]
    public class pesaje : Base
    {
        #region "Propiedades"
        public long? id { get; set; }
        public long gkey { get; set; }
        public string container { get; set; }
        public string peso { get; set; }
        public bool estado { get; set; }
        public string ip { get; set; }
        public string mensaje { get; set; }
        public string usuarioCrea { get; set; }
        public DateTime? fechaCreacion { get; set; }
        public string usuarioModifica { get; set; }
        public DateTime? fechaModifica { get; set; }
        #endregion
    }
}
