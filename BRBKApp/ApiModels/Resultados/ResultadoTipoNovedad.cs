using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiModels.Resultados
{
    [Serializable]
    public class ResultadoTipoNovedad : Base
    {
        #region "Propiedades"     


        public Int64 Id { get; set; }
        public string Name { get; set; }

        public bool Generate_notification { get; set; }
        #endregion

    }
}
