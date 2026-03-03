using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiModels.Resultados
{
    [Serializable]
    public class ResultadoWorkInProgressRegisters : Base
    {
        #region "Propiedades"     
        public Int64 Id { get; set; }
        public string Task_name { get; set; }
        public string Block_name { get; set; }
        public string Container { get; set; }
        public string Temperature { get; set; }
        public string Ventilation { get; set; }
        public string Commentary { get; set; }
        public Int64? WorkInProgressId { get; set; }

        public string Photo1 { get; set; }
        public string Photo2 { get; set; }
        public string Photo3 { get; set; }
        public string Photo4 { get; set; }

        #endregion

    }
}
