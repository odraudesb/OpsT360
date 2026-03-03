using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiModels.AppModels
{
    [Serializable]
    public class nave : Base
    {
        #region "Propiedades"
        public string id { get; set; }
        public string name { get; set; }
        public string in_customs_voy_nbr { get; set; }
        public DateTime published_eta { get; set; }
        public DateTime? ata{ get; set; }
        #endregion
    }
}
