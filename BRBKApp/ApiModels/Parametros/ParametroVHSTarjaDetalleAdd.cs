using ApiModels.AppModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiModels.Parametros
{
    public class ParametroVHSTarjaDetalleAdd : Base
    {
        public int TarjaID { get; set; }
        public string TipoCargaDescripcion { get; set; }
        public string InformacionVehiculo { get; set; }
        public string UbicacionBodega { get; set; }
        public string DocumentoTransporte { get; set; }
        public string PackingList { get; set; }
        public string VIN {  get; set; }
        public string NumeroMotor { get; set; }
        public string Observaciones { get; set; }
        public List<VHSTarjaDetalleFoto> Fotos { get; set; }
    }
}
