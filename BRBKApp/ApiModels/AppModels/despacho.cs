using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiModels.AppModels
{
    [Serializable]
    public class despacho : Base
    {
        #region "Propiedades"
        public long? idDespacho { get; set; }
        public long idTarjaDet { get; set; }
        public string pase { get; set; }
        public tarjaDet tarjaDet { get; set; }
        public string bl { get; set; }
        public string mrn { get; set; }
        public string msn { get; set; }
        public string hsn { get; set; }
        public string placa { get; set; }
        public string idchofer { get; set; }
        public string chofer { get; set; }
        public decimal cantidad { get; set; }
        public string observacion { get; set; }
        public string estado { get; set; }
        public string usuarioCrea { get; set; }
        public DateTime? fechaCreacion { get; set; }
        public string usuarioModifica { get; set; }
        public DateTime? fechaModifica { get; set; }
        public List<fotoDespacho> Fotos { get; set; }
        public string delivery { get; set; }
        public long PRE_GATE_ID { get; set; }
        public string SMDT_xml { get; set; }
        #endregion
    }
}
