using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace N4Ws.Entidad
{
    public class BreakBulkLoadTruck
    {
        public string BL { get; set; }
        public string PLACA { get; set; }
        public string CANTIDAD { get; set; }

        public string USER { get; set; }

        public BreakBulkLoadTruck(string _bl, string _placa, string _cantidad, string _user)
        {
            this.BL = _bl.Trim();
            this.PLACA = _placa.Trim();
            this.CANTIDAD = _cantidad;
            this.USER = _user;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("<groovy class-location=\"code-extension\" class-name=\"CGSABillOfLadingLoadTruck\">");

            sb.Append("<parameters>");

            if (!string.IsNullOrEmpty(this.BL))
            {
                sb.AppendFormat("<parameter id=\"BL\" value=\"{0}\" />", this.BL.Trim());
            }
            if (!string.IsNullOrEmpty(this.PLACA))
            {
                sb.AppendFormat("<parameter id=\"TRUCKID\" value=\"{0}\" />", this.PLACA.Trim());
            }
            if (!string.IsNullOrEmpty(this.CANTIDAD))
            {
                sb.AppendFormat("<parameter id=\"QTY\" value=\"{0}\" />", this.CANTIDAD.Trim());
            }
            if (!string.IsNullOrEmpty(this.USER))
            {
                sb.AppendFormat("<parameter id=\"USER\" value=\"{0}\" />", this.USER.Trim());
            }

            sb.Append("</parameters>");
            sb.Append("</groovy>");
            return sb.ToString();
        }
    }
}
