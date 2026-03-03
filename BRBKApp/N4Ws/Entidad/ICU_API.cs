using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace N4Ws.Entidad
{
    [XmlRoot("icu")]
    public class ICU_API
    {
        [XmlArray("units")]
        [XmlArrayItem("unit-identity", typeof(ICU_UNIT))]
        public List<ICU_UNIT> units { get; set; }

        [XmlArray("properties")]
        [XmlArrayItem("property", typeof(ICU_PROPERTY))]
        public List<ICU_PROPERTY> properties { get; set; }


        [XmlElement("event")]
        public ICU_EVENT evento { get; set; }
    }

    public class ICU_UNIT
    {
        [XmlAttribute("id")]
        public string id { get; set; }
        [XmlAttribute("type")]
        public string type { get; set; }
    }

    public class ICU_PROPERTY
    {
        [XmlAttribute("tag")]
        public string tag { get; set; }
        [XmlAttribute("value")]
        public string value { get; set; }
    }

    public class ICU_EVENT
    {
        [XmlAttribute("id")]
        public string id { get; set; }
        [XmlAttribute("note")]
        public string note { get; set; }

        [XmlAttribute("time-event-applied")]
        public string timeeventapplied { get; set; }
        [XmlAttribute("user-id")]
        public string userid { get; set; }
    }

}
