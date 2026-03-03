using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace N4Ws.Entidad
{
    public class CGSAComputeReeferExpoWS
    {
        public string unitid { get; set; }
        public string line { get; set; }
        public category categoria { get; set; }
        public string referencia { get; set; }
        public float qty { get; set; }
        public Int64 gkey { get; set; }


        public enum category
        {
            IMPRT, EXPRT
        }


        public CGSAComputeReeferExpoWS(string unit, string line, string reference, category cat, Int64 ggky, float hhour)
        {
            this.unitid = unit; this.line = line; this.referencia = reference; this.categoria = cat;this.gkey = ggky; this.qty = hhour;
           
        }
        public CGSAComputeReeferExpoWS( )
        {

        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("<groovy class-name=\"CGSAComputeReeferExpoWS\" class-location=\"code-extension\"><parameters>");
            sb.AppendFormat("<parameter id=\"{0}\" value=\"{1}\" />", nameof(gkey), gkey);
            sb.AppendFormat("<parameter id=\"{0}\" value=\"{1}\" />", nameof(line), line);
            sb.AppendFormat("<parameter id=\"{0}\" value=\"{1}\" />", nameof(categoria), categoria);
            sb.AppendFormat("<parameter id=\"{0}\" value=\"{1}\" />", nameof(referencia), referencia);
            sb.AppendFormat("<parameter id=\"{0}\" value=\"{1}\" />", nameof(qty), qty);
            sb.Append("</parameters></groovy>");
            return sb.ToString();
        }
        /*
     <groovy class-name="CGSAComputeReeferExpoWS" class-location="code-extension">
<parameters>
	<parameter id="gkey" value="4286027" />
	<parameter id="line" value="MSK" />
	<parameter id="qty" value="11.44" />
	<parameter id="unitid" value="MNBU3294284" />
	<parameter id="referencia" value="MSK2018073" />
	<parameter id="categoria" value="EXPRT" />
</parameters>
</groovy>

         
         */
    }
}
