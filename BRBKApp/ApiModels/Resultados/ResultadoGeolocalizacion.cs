using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using Microsoft.SqlServer.Types;

namespace ApiModels.Resultados
{
    [Serializable]
    public class ResultadoGeolocalizacion : Base
    {
        #region "Propiedades"     

        public Int64 Id { get; set; }

        //public Microsoft.SqlServer.Types.SqlGeography Geom { get; set; }


        #endregion

    }
}
