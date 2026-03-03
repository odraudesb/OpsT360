using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace MiWebApi
{
    [Serializable]
    public class ResultadoOpciones:Base
    {
        #region "Propiedades"     

  
        public Int64 Id { get; set; }
        
        public string RoleName { get; set; }
       
        public Int64 OptionId { get; set; }
     
        public string OptionName { get; set; }
   
        public string Form { get; set; }
      
        public int? Order { get; set; }

        #endregion
    }
}