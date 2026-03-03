using System.Collections.Generic;

namespace ApiModels.AppModels
{
    public class AppModelVHSTarjaDetalle
    {
        public long DetalleTarjaID { get; set; }
        public int TarjaID { get; set; }
        public string TipoCargaDescripcion { get; set; }
        public string InformacionVehiculo { get; set; }
        public string UbicacionBodega { get; set; }
        public string DocumentoTransporte { get; set; }
        public string PackingList { get; set; }
        public string VIN { get; set; }
        public string NumeroMotor { get; set; }
        public string Observaciones { get; set; }
        public List<VHSTarjaDetalleFoto> Fotos { get; set; }
        public int Id { get; set; }
        public int NumeroBloque { get; set; }


        public string nombre_bloque { get; set; }
    }
}