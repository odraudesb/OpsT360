using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiModels.AppModels
{
    [Serializable]
    public class recepcion : Base
    {
        #region "Propiedades"                                                   
        public long? idRecepcion { get; set; }
        public long idTarjaDet { get; set; }
        public tarjaDet TarjaDet { get; set; }
        public int idGrupo { get; set; }
        public grupos Grupo { get; set; }
        public string lugar { get; set; }
        public decimal? cantidad { get; set; }
        public string ubicacion { get; set; }
        public ubicacion Ubicaciones { get; set; }
        public string observacion { get; set; }
        public string estado { get; set; }
        public estados Estados { get; set; }
        public string usuarioCrea { get; set; }
        public DateTime? fechaCreacion { get; set; }
        public string usuarioModifica { get; set; }
        public DateTime? fechaModifica { get; set; }

        public List<fotoRecepcion> Fotos { get; set; }

        #endregion
    }
}
