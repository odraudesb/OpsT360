using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace N4Ws.Entidad
{
    public class GroovyPOW
    {
        public string POW { get; set; }
        public string VISIT { get; set; }
        public string UNIT { get; set; }
        public string POSITION { get; set; }
        public string SEAL { get; set; }
        public string USER { get; set; }

        public GroovyPOW(string _pow, string _visit, string _unit, string _position,  string _seal ,  string _user)
        {
            this.POW = _pow.Trim();
            this.VISIT = _visit.Trim();
            this.UNIT = _unit;
            this.POSITION = _position;
            this.SEAL = _seal;
            this.USER = _user;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("<groovy class-location=\"code-extension\" class-name=\"CGSAUnitLoad\">");

            sb.Append("<parameters>");

            if (!string.IsNullOrEmpty(this.POW))
            {
                sb.AppendFormat("<parameter id=\"POW\" value=\"{0}\" />", this.POW.Trim());
            }
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
            //if (!string.IsNullOrEmpty(this.SEAL))
            //{
            //    sb.AppendFormat("<parameter id=\"SEAL\" value=\"{0}\" />", this.SEAL.Trim());
            //}


            sb.Append("</parameters>");
            sb.Append("</groovy>");
            return sb.ToString();
        }
    }
}
