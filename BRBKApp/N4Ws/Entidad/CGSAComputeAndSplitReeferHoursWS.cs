using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace N4Ws.Entidad
{
    public class CGSAComputeAndSplitReeferHoursWS
    {
        public string UNIT { get; set; }
        public string IPTD { get; set; }
        public string CATEGORY { get; set; }
        public string VESSELVISIT { get; set; }
        public string LINEOP { get; set; }
        public string MODE { get; set; }

        public enum mode
        {
            UPDATE
        }

        public enum category
        {
            IMPRT, EXPRT, TRSH
        }


        public CGSAComputeAndSplitReeferHoursWS(string unit, string iptd, string vesselVisit)
        {
            this.UNIT = unit; this.IPTD = iptd; this.VESSELVISIT = vesselVisit;
            this.LINEOP = "N"; this.CATEGORY = category.IMPRT.ToString();
        }



        public CGSAComputeAndSplitReeferHoursWS(string unit, string iptd, string vesselVisit, mode _mode, category cat )
        {
            this.UNIT = unit; this.IPTD = iptd; this.VESSELVISIT = vesselVisit;
            this.LINEOP = "N"; this.MODE = _mode.ToString(); this.CATEGORY = category.IMPRT.ToString();
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("<groovy class-name=\"CGSAComputeAndSplitReeferHoursWS\" class-location=\"code-extension\"><parameters>");

            sb.AppendFormat("<parameter id=\"{0}\" value=\"{1}\" />", nameof(UNIT), UNIT);

            sb.AppendFormat("<parameter id=\"{0}\" value=\"{1}\" />", nameof(IPTD), IPTD);

            sb.AppendFormat("<parameter id=\"{0}\" value=\"{1}\" />", nameof(CATEGORY), CATEGORY);

            sb.AppendFormat("<parameter id=\"{0}\" value=\"{1}\" />", nameof(VESSELVISIT), VESSELVISIT);

            sb.AppendFormat("<parameter id=\"{0}\" value=\"{1}\" />", nameof(LINEOP), LINEOP);
            if (!string.IsNullOrEmpty(MODE))
            {
                sb.AppendFormat("<parameter id=\"{0}\" value=\"{1}\" />", nameof(MODE), MODE);
            }
            sb.Append("</parameters></groovy>");
            return sb.ToString();
        }
        /*
         <groovy class-name="CGSAComputeAndSplitReeferHoursWS" class-location="code-extension"><parameters>
    <parameter id="UNIT" value="4284769" />
    <parameter id="IPTD" value="2019-12-23 15:00:00" />
    <parameter id="CATEGORY" value="IMPO" />
    <parameter id="VESSELVISIT" value="MSK2018073" />
    <parameter id="LINEOP" value="N" />
    <parameter id="MODE" value="UPDATE" />
  </parameters></groovy>

          <parameters>
    <parameter id="UNIT" value="4284769" />
    <parameter id="IPTD" value="2019-12-23 15:00:00" />
    <parameter id="CATEGORY" value="IMPO" />
    <parameter id="VESSELVISIT" value="MSK2018073" />
    <parameter id="LINEOP" value="N" />
  </parameters>
</groovy>

         
         */
    }
}
