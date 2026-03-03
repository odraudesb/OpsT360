using System;

namespace ApiModels.AppModels
{
    [Serializable]
    public class BAN_Capacidad_Hora_Bodega : Base
    {
        #region "Propiedades"
        public long? id { get; set; }
        public string idNave { get; set; }
        public string nave { get; set; }
        public int? idHoraInicio { get; set; }
        public string horaInicio { get; set; }
        public int? idHoraFin { get; set; }
        public string horaFin { get; set; }
        public int? idBodega { get; set; }
        public int? idBloque { get; set; }
        public int? box { get; set; }
        public int? disponible { get; set; }
        public bool? estado { get; set; }
        public string usuarioCrea { get; set; }
        public DateTime? fechaCreacion { get; set; }
        public string usuarioModifica { get; set; }
        public DateTime? fechaModifica { get; set; }
        public BAN_Catalogo_Bloque oBloque { get; set; }
        #endregion

       
    }
}

