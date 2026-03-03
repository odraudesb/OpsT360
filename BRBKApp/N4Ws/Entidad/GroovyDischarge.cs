using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace N4Ws.Entidad
{
    public class GroovyDischarge
    {
        public string VISIT { get; set; }
        public string UNIT { get; set; }
        public string POSITION { get; set; }
        public string USER { get; set; }

        public GroovyDischarge( string _visit, string _unit, string _position,  string _user)
        {
            this.VISIT = _visit.Trim();
            this.UNIT = _unit;
            this.POSITION = _position;
            this.USER = _user;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("<groovy class-location=\"code-extension\" class-name=\"CGSAUnitDischarge\">");

            sb.Append("<parameters>");

            if (!string.IsNullOrEmpty(this.VISIT))
            {
                sb.AppendFormat("<parameter id=\"VISIT\" value=\"{0}\" />", this.VISIT.Trim());
            }
            if (!string.IsNullOrEmpty(this.UNIT))
            {
                sb.AppendFormat("<parameter id=\"UNIT\" value=\"{0}\" />", this.UNIT.Trim());
            }
            if (!string.IsNullOrEmpty(this.POSITION))
            {
                sb.AppendFormat("<parameter id=\"POSITION\" value=\"{0}\" />", this.POSITION.Trim());
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
