using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace N4Ws.Entidad
{
    internal enum _Action
    {
        RELEASE_HOLD, APPLY_HOLD
    }
    internal class hpu
    {
        public List<IEntity> entities { get; set; }
        public List<flag> flags { get; set; }

        public hpu()
        {
            this.entities = new List<IEntity>();
            this.flags = new List<flag>();
        }

        public override string ToString()
        {
            StringBuilder bb = new StringBuilder();
            bb.Append("<hpu>");
            if (this.entities.Count > 0)
            {
                bb.Append("<entities>");
                bb.Append("<units>");
                entities.ForEach(n => { bb.Append(n.ToString()); });
                bb.Append("</units>");
                bb.Append("</entities>");
            }
            if (this.flags.Count > 0)
            {
                bb.Append("<flags>");
                flags.ForEach(f => {bb.Append(f.ToString()); });
                bb.Append("</flags>");
            }
            bb.Append("</hpu>");
            return bb.ToString();
        }
    }



    internal class unit :IEntity
    {
        public string id { get; set; }

        public override string ToString()
        {
            return string.Format("<unit id=\"{0}\" />",this.id);
        }
    }

    internal class flag
    {
        public string hold_perm_id { get; set; }
        public _Action action { get; set; }
        public override string ToString()
        {
            StringBuilder bb = new StringBuilder();
            bb.AppendFormat("<flag hold-perm-id=\"{0}\" action=\"{1}\"/>", this.hold_perm_id, this.action);
            return bb.ToString();
        }
    }



   internal interface IEntity
    {
        string id { get; set; }
        
    }

   

}
