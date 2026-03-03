using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace N4Ws.Entidad
{
    public class GroovyCodeExtension
    {

        public string name { get; set; }
        public string location { get; set; }
        public Dictionary<string,string> parameters { get; set; }
        public GroovyCodeExtension()
        {
            this.parameters = new Dictionary<string, string>();
            this.location = "code-extension"; 
        }

         public GroovyCodeExtension(string _name)
        {
            this.name = _name;
            this.location = "code-extension";
        }   
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendFormat(" <groovy class-name=\"{0}\" class-location=\"{1}\">", this.name, this.location);
            if (this.parameters != null && this.parameters.Count > 0)
            {
                sb.Append("<parameters>");
                foreach (var pi in parameters)
                {
                    sb.AppendFormat("<parameter id=\"{0}\" value=\"{1}\" />",pi.Key, pi.Value);
                }
                sb.Append("</parameters>");
            }
            sb.Append("</groovy>");
            return sb.ToString();
        }

    }

    /*
   


     
     */
}
