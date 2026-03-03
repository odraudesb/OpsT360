using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiModels.AppModels
{
    [Serializable]
    public class novedad : Base
    {
        #region "Propiedades"
        public long? idNovedad { get; set; }
        public long idRecepcion { get; set; }
        public recepcion Recepcion { get; set; }
        public DateTime? fecha { get; set; }
        public string descripcion { get; set; }
        public string estado { get; set; }
        public estados Estados { get; set; }
        public string usuarioCrea { get; set; }
        public DateTime? fechaCreacion { get; set; }
        public string usuarioModifica { get; set; }
        public DateTime? fechaModifica { get; set; }
        public List<fotoNovedad> Fotos { get; set; }
        #endregion
    }
}
