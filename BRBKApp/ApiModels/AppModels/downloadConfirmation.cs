using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiModels.AppModels
{
    [Serializable]
    public class downloadConfirmation : Base
    {
        #region "Propiedades"
        public long? id { get; set; }
        public long? gkey { get; set; }
        public string container { get; set; }
        public string dataContainer { get; set; }
        public string position { get; set; }
        public string referencia { get; set; }
        public string xmlN4 { get; set; }
        public string respuestaN4 { get; set; }

        public bool estado { get; set; }
        public string usuarioCrea { get; set; }
        
        #endregion
    }

    
}
