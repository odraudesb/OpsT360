using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiModels.AppModels
{
    public  class pasePuerta : Base
    {
        public long? idTarjaDet { get; set; }
        public string pase { get; set; }
        public tarjaDet tarjaDet { get; set; }
        public string bl { get; set; }
        public string mrn { get; set; }
        public string msn { get; set; }
        public string hsn { get; set; }
        public string placa { get; set; }
        public string idchofer { get; set; }
        public string chofer { get; set; }
        public int cantidad { get; set; }
        public string delivery { get; set; }
        public long PRE_GATE_ID { get; set; }
    }
}
