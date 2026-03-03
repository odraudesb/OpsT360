using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiModels.Parametros
{
    public class ParametroRegistrarRecepcionAisv : Base
    {
        public long? idMovimiento { get; set; }
        public long idStowageAisv { get; set; }

        public int idModalidad { get; set; }
        public string tipo { get; set; }
        public int cantidad { get; set; }
        public string observacion { get; set; }
        public string estado { get; set; }

        public List<ParametroRegistrarRecepcionFotoAisv> Fotos { get; set; }
    }

    public class ParametroRegistrarRecepcionFotoAisv : Base
    {
        public byte[] foto { get; set; }
        public string ruta { get; set; }
        public string estado { get; set; }

    }

    public class ParametroActualizarRecepcionAisv : Base
    {
        public long idStowageAisv { get; set; }
        public long? idMovimiento { get; set; }
        public string tipo { get; set; }
        public int? idUbicacion { get; set; }
        public bool isMix { get; set; }
        public string referencia { get; set; }
    }

    public class ParametroAnularRecepcionAisv : Base
    {
        public long? idMovimiento { get; set; }
    }
}
