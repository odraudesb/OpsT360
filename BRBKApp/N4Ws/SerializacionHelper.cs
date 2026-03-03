using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace  N4Ws
{
    public class SerializacionHelper
    {

        /// <summary>
        /// Convierte una entidad a XML (string)
        /// </summary>
        /// <typeparam name="T">Es el tipo del objeto o.GetType() / tipeof(T)</typeparam>
        /// <param name="obj">Entidad (T)</param>
        /// <returns>Candena de texto como XML</returns>
        public static string XmlSerializeEntity<T>(T obj, Action<Exception, T> OnError = null)
        {
            string xmlStr = String.Empty;
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = false;
            settings.OmitXmlDeclaration = true;
            settings.NewLineChars = String.Empty;
            settings.NewLineHandling = NewLineHandling.None;
            //omita los caracteres especiales
            settings.CheckCharacters = false;
            try
            {
                using (StringWriter stringWriter = new StringWriter())
                {
                    using (XmlWriter xmlWriter = XmlWriter.Create(stringWriter, settings))
                    {
                        XmlSerializer serializer = new XmlSerializer(obj.GetType());
                        serializer.Serialize(xmlWriter, obj);
                        xmlStr = stringWriter.ToString();
                        xmlWriter.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                //llame a un metodo que grabe log, le entrego el objeto completo
                //y le entrego el error
                OnError?.Invoke(ex, obj);
                return null;
            }

            return xmlStr;
        }
        /// <summary>
        /// Convierte XML en una entidad tipada
        /// </summary>
        /// <typeparam name="T">Es el tipo del objeto o.GetType() / tipeof(T)</typeparam>
        /// <param name="data">XML que contiene la entidad</param>
        /// <returns>Nueva instancia de T</returns>
        public static T XmlDeserializeEntity<T>(string data, Action<Exception, string> OnError = null) where T : class
        {
            //aqui debe venir escpados los caracteres especiales
            try
            {
                XmlSerializer xmlSer = new XmlSerializer(typeof(T));
                StringReader reader = new StringReader(data);
                return (T)(xmlSer.Deserialize(reader));
            }
            catch (Exception ex)
            {
                OnError?.Invoke(ex, data);
                //llame a un metodo que grabe log, le envio el xml pasado
                //le envio el error.
                return null;
            }

        }
    }
}
