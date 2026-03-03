using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiModels.Resultados
{
    [Serializable]
    public class ResultadoNovedad : Base
    {
        #region "Propiedades"     
        public Int64 Id { get; set; }
        public string Note { get; set; }
        public string Photo1 { get; set; }
        public string Photo2 { get; set; }
        public string Photo3 { get; set; }
        public string Photo4 { get; set; }

        public DateTime? Reading_date { get; set; }
        public DateTime? Expiration_date { get; set; }

        public Int64? TypeNoveltyId { get; set; }
        public Int64? WorkInProgressRegisterId { get; set; }

        public Int64? UserId { get; set; }
        #endregion

    }
}
