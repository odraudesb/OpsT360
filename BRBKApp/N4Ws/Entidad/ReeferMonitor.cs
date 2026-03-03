using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace N4Ws.Entidad
{
    public class ReeferMonitor
    {
        public string UNIT { get; set; }
        public string TIPO { get; set; }

        public string O2 { get; set; }
        public string CO2 { get; set; }
        public string HUMIDITY { get; set; }
        public string VENTILATION { get; set; }

        public ReeferMonitor(string unit, string tipo, string o2, string co2, string humidity, string ventilation)
        {
            this.UNIT = unit.Trim(); 
            this.TIPO = tipo.Trim();
            this.O2 = o2;
            this.CO2 = co2;
            this.HUMIDITY = humidity;
            this.VENTILATION = ventilation;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("<argo:reefer-monitor xmlns:argo=\"http://www.navis.com/argo\"><update-reefers><reefer-monitoring>");

            sb.AppendFormat("<unit-identity id=\"{0}\"/>",  UNIT);

            sb.AppendFormat("<reefer is-power=\"{0}\" wanted-is-power=\"{0}\" ",  TIPO);
            if (!string.IsNullOrEmpty(this.O2)) 
            {
                sb.AppendFormat("o2-pct=\"{0}\" ", this.O2.Trim());
            }
            if (!string.IsNullOrEmpty(this.CO2))
            {
                sb.AppendFormat("co2-pct=\"{0}\" ", this.CO2.Trim());
            }
            if (!string.IsNullOrEmpty(this.HUMIDITY))
            {
                sb.AppendFormat("humidity-pct=\"{0}\" ", this.HUMIDITY.Trim());
            }
            if (!string.IsNullOrEmpty(this.VENTILATION))
            {
                sb.AppendFormat("vent-required-value=\"{0}\" vent-required-unit=\"PERCENTAGE\" ", this.VENTILATION.Trim());
            }

            sb.AppendFormat("/>");
            sb.Append("</reefer-monitoring></update-reefers></argo:reefer-monitor>");
            return sb.ToString();
        }

    }
}
