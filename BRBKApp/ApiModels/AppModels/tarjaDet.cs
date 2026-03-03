using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiModels.AppModels
{
    [Serializable]
    public class tarjaDet : Base
    { 

    #region "Propiedades"
        public long? idTarjaDet { get; set; }
        public long? idTarja { get; set; }

        public string idNave { get; set; }
        public string idAgente { get; set; }
        public string Agente { get; set; }
        public decimal? arrastre { get; set; }
        public decimal? pendiente { get; set; }

        public tarjaCab tarjaCab { get; set; }
        public string bl { get; set; }
        public string mrn { get; set; }
        public string msn { get; set; }
        public string hsn { get; set; }
        public string idConsignatario { get; set; }
        public string Consignatario { get; set; }
        public string carrier_id { get; set; }
        public string productoEcuapass { get; set; }
        public int idProducto { get; set; }
        public productos producto { get; set; }
        public int idManiobra { get; set; }
        public maniobra maniobra { get; set; }
        public int? idItem { get; set; }
        public int idCondicion { get; set; }
        public condicion condicion { get; set; }
        public decimal cantidad { get; set; }
        public int kilos { get; set; }
        public decimal? cubicaje { get; set; }
        public string descripcion { get; set; }
        public string contenido { get; set; }
        public string observacion { get; set; }
        public decimal? tonelaje { get; set; }
        public string ubicacion { get; set; }
        public ubicacion Ubicaciones { get; set; }
        public string estado { get; set; }
        public estados Estados { get; set; }

        public string carga { get; set; }
        public string consigna { get; set; }

        public bool imdt { get; set; }
        public string imdt_num { get; set; }
        public DateTime fecha_imdt { get; set; }
        public string usuario_imdt { get; set; }
        public bool n4 { get; set; }
        public DateTime fecha_n4 { get; set; }
        public string usuario_n4 { get; set; }
        public string imo { get; set; }

        public string usuarioCrea { get; set; }
        public DateTime? fechaCreacion { get; set; }
        public string usuarioModifica { get; set; }
        public DateTime? fechaModifica { get; set; }
        #endregion

    }
}
