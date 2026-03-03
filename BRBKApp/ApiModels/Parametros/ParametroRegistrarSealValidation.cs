using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiModels.Parametros
{
    public class ParametroRegistrarSealMuelle : Base
    {
        public string container { get; set; }
        public string sello_CGSA { get; set; }
        public string sello1 { get; set; }
        public string sello2 { get; set; }
        public string sello3 { get; set; }
        public string sello4 { get; set; }
        public string ip { get; set; }
        public string color { get; set; }

        public long gkey { get; set; }
        public string dataContainer { get; set; }
        public string position { get; set; }
        public string referencia { get; set; }
        public string xmlN4 { get; set; }
        public string respuestaN4 { get; set; }

        public List<ParametroRegistrarSelloFoto> Fotos { get; set; }
    }

    public class ParametroRegistrarSealPreEmbarque : Base
    {
        public string container { get; set; }
        public string sello_CGSA { get; set; }
        public string sello1 { get; set; }
        public string sello2 { get; set; }
        public string sello3 { get; set; }
        public string ip { get; set; }

        public long gkey { get; set; }
        public string mensaje { get; set; }

        public List<ParametroRegistrarSelloFoto> Fotos { get; set; }
    }

    public class ParametroRegistrarSealAssignsExpo : Base
    {
        public string container { get; set; }
        public string sello_CGSA { get; set; }
        public string sello1 { get; set; }
        public string sello2 { get; set; }
        public string sello3 { get; set; }
        public string sello4 { get; set; }
        public string ip { get; set; }

        public long gkey { get; set; }
        public string mensaje { get; set; }

        public List<ParametroRegistrarSelloFoto> Fotos { get; set; }
    }

    public class ParametroRegistrarSealValidationYard : Base
    {
        public string container { get; set; }
        public string sello_CGSA { get; set; }
        public string ip { get; set; }

        public long gkey { get; set; }
        public string mensaje { get; set; }

        public List<ParametroRegistrarSelloFoto> Fotos { get; set; }
    }
}
