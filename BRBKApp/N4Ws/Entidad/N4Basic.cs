using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web.Services.Protocols;
using N4Ws.N4Esquema;
using System.Xml.Serialization;
using N4Ws.Entidad;
using AccesoDatos;


namespace N4Ws.Entidad
{
    internal class N4Basic : ArgobasicService
    {
        private string _b64Credentials;
        string _username = string.Empty;
        string _password = string.Empty;
        const string _scope = "ICT/ECU/GYE/CGSA";
        public string scope { get; set; }
        protected override System.Net.WebRequest GetWebRequest(Uri uri)
        {
            System.Net.HttpWebRequest httpRequest = (System.Net.HttpWebRequest)base.GetWebRequest(uri);
            httpRequest.Headers.Add("Authorization", "Basic " + _b64Credentials);
            return (System.Net.WebRequest)httpRequest;
        }
        private N4Basic(string _user, string _password, string _url, string _scope = null)
        {
            byte[] bCredentials = Encoding.ASCII.GetBytes(_user + ":" + _password);
            _b64Credentials = Convert.ToBase64String(bCredentials);
            this.Url = _url;
            this.scope = string.IsNullOrEmpty(_scope) ? this.scope : _scope;
        }
        private static N4Basic instance = null;
        public static N4Basic GetInstance(string us, string pass, string url, string sco = null)
        {
            if (instance == null)
                instance = new N4Basic(us, pass, url, sco);
            return instance;
        }
        public N4_BasicResponse BasicInvokeService(string xmlDoc, string oclass, string omethod, string ouser, int timer = 6000)
        {
            N4_BasicResponse n4_r = new N4_BasicResponse();
            DateTime fecha_inicio = DateTime.Now;

            string resp = string.Empty; ;
             var ss = System.Configuration.ConfigurationManager.ConnectionStrings["BRBK"]?.ConnectionString;
            try
            {
                basicInvoke bp = new basicInvoke();
                bp.scopeCoordinateIds = _scope;
                XDocument xds = XDocument.Parse(xmlDoc);
                bp.xmlDoc = xmlDoc;
                int tim = timer > 0 ? timer : 3000;
                
                resp = this.basicInvoke(bp).basicInvokeResponse1;

                if (resp != null)
                {
                    xds = XDocument.Parse(resp);
                    //toda la respuesta de N4
                    n4_r.response = xds;
#if DEBUG
                    // string xf = "<argo-response status=\"2\" status-id=\"WARNING\"><messages><message message-id=\"INFO\" message-severity=\"INFO\" message-text=\"[id:MSCU3551446]\" message-detail=\"[id:MSCU3551446]\" /></messages><argo:snx-response xmlns:argo=\"http://www.navis.com/argo\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\"><status>OZ</status></argo:snx-response></argo-response>";
                    //    string xf = "<argo-response status=\"2\" status-id=\"WARNING\"><messages><message message-id=\"INFO\" message-severity=\"INFO\" message-text=\"[id:MSCU3551446]\" message-detail=\"[id:MSCU3551446]\" /><message message-id=\"WARNING_ISO_NOT_UPDATED\" message-severity=\"WARNING\" message-text=\"Equipment Type cannot be changed for MSCU3551446 from 22G1 to 2200 as the existing equipment type 22G1 was created at the IN_GATE which cannot be updated by the data from the SNX.\" message-detail = \"Equipment Type cannot be changed for MSCU3551446 from 22G1 to 2200 as the existing equipment type 22G1 was created at the IN_GATE which cannot be updated by the data from the SNX.\" /></messages><argo:snx-response xmlns:argo=\"http://www.navis.com/argo\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\"><status>OZ</status></argo:snx-response></argo-response>";
                    //    xds = XDocument.Parse(xf);
#endif
                    n4_r.status = (xds.Root.Attributes("status").FirstOrDefault() != null) ? int.Parse(xds.Root.Attributes("status").FirstOrDefault().Value) : -1;
                    n4_r.status_id = xds.Root.Attributes("status-id").FirstOrDefault()?.Value;

                    var snx_status = xds.Descendants().Where(u => u.Name.LocalName.Equals("status")).FirstOrDefault()?.Value;
                    if (!string.IsNullOrEmpty(snx_status))
                    {
                        n4_r.snx_response = new Snxresponse() { status = snx_status };
                    }

                    var messages = xds.Root.Descendants("messages")?.Descendants("message");
                    if (messages?.Count() > 0)
                    {
                        foreach (var message in messages)
                        {
                            n4_r.messages.Add(new N4_response_message(message.Attribute("message-id")?.Value, message.Attribute("message-severity")?.Value, message.Attribute("message-text")?.Value));
                        }
                    }
                    n4_r.trace = String.IsNullOrEmpty(ss) ? 0 : N4Traza(ss, ouser, oclass, omethod, xmlDoc, resp,fecha_inicio,"WITHOUT");
                    return n4_r;
                }
                n4_r.status = 3;
                n4_r.messages.Add(new N4_response_message("NO_RESPONSE", "CRITICAL", "N4 RESPONSE ES NULL"));
            }
            catch (System.Xml.XmlException e)
            {
                n4_r.status = 3;
                n4_r.messages.Add(new N4_response_message("XML_ERROR", "CRITICAL", e.Message));
            }
            catch (SoapException e)
            {
                n4_r.status = 3;
                n4_r.messages.Add(new N4_response_message("SOAP_ERROR", "CRITICAL", e.Message));
            }
            catch (TimeoutException e)
            {
                n4_r.status = 3;
                n4_r.messages.Add(new N4_response_message("TIMEOUT_ERROR", "CRITICAL", e.Message));
            }
            catch (WebException e)
            {
                n4_r.status = 3;
                n4_r.messages.Add(new N4_response_message("WEB_ERROR", "CRITICAL", e.Message));
            }
            catch (Exception e)
            {
                n4_r.status = 3;
                n4_r.messages.Add(new N4_response_message("CODE_ERROR", "CRITICAL", e.Message));
            }
            //trazar todo lo que hagan en N4, usuario, clase, metodo, parametros, respuesta
              n4_r.trace = String.IsNullOrEmpty(ss)?0: N4Traza(ss,ouser,oclass,omethod,xmlDoc,resp,fecha_inicio,"WITHOUT");
            return n4_r;
        }

        public N4_BasicResponse BasicInvokeService(string xmlDoc, string oclass, string omethod, string ouser, int timer = 6000, DateTime? fecha_inicio=null, string palabra_clave=null)
        {
            N4_BasicResponse n4_r = new N4_BasicResponse();
            fecha_inicio = fecha_inicio.HasValue ? fecha_inicio.Value : DateTime.Now;
            palabra_clave = string.IsNullOrEmpty(palabra_clave) ? "No incluida" : palabra_clave.Trim().ToUpper();

            string resp = string.Empty; ;
            var ss = System.Configuration.ConfigurationManager.ConnectionStrings["BRBK"]?.ConnectionString;
            try
            {
                basicInvoke bp = new basicInvoke();
                bp.scopeCoordinateIds = _scope;
                XDocument xds = XDocument.Parse(xmlDoc);
                bp.xmlDoc = xmlDoc;
                int tim = timer > 0 ? timer : 3000;


                resp = this.basicInvoke(bp).basicInvokeResponse1;

                if (resp != null)
                {
                    xds = XDocument.Parse(resp);
                    //toda la respuesta de N4
                    n4_r.response = xds;
#if DEBUG
                    // string xf = "<argo-response status=\"2\" status-id=\"WARNING\"><messages><message message-id=\"INFO\" message-severity=\"INFO\" message-text=\"[id:MSCU3551446]\" message-detail=\"[id:MSCU3551446]\" /></messages><argo:snx-response xmlns:argo=\"http://www.navis.com/argo\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\"><status>OZ</status></argo:snx-response></argo-response>";
                    //    string xf = "<argo-response status=\"2\" status-id=\"WARNING\"><messages><message message-id=\"INFO\" message-severity=\"INFO\" message-text=\"[id:MSCU3551446]\" message-detail=\"[id:MSCU3551446]\" /><message message-id=\"WARNING_ISO_NOT_UPDATED\" message-severity=\"WARNING\" message-text=\"Equipment Type cannot be changed for MSCU3551446 from 22G1 to 2200 as the existing equipment type 22G1 was created at the IN_GATE which cannot be updated by the data from the SNX.\" message-detail = \"Equipment Type cannot be changed for MSCU3551446 from 22G1 to 2200 as the existing equipment type 22G1 was created at the IN_GATE which cannot be updated by the data from the SNX.\" /></messages><argo:snx-response xmlns:argo=\"http://www.navis.com/argo\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\"><status>OZ</status></argo:snx-response></argo-response>";
                    //    xds = XDocument.Parse(xf);
#endif
                    n4_r.status = (xds.Root.Attributes("status").FirstOrDefault() != null) ? int.Parse(xds.Root.Attributes("status").FirstOrDefault().Value) : -1;
                    n4_r.status_id = xds.Root.Attributes("status-id").FirstOrDefault()?.Value;

                    var snx_status = xds.Descendants().Where(u => u.Name.LocalName.Equals("status")).FirstOrDefault()?.Value;
                    if (!string.IsNullOrEmpty(snx_status))
                    {
                        n4_r.snx_response = new Snxresponse() { status = snx_status };
                    }

                    var messages = xds.Root.Descendants("messages")?.Descendants("message");
                    if (messages?.Count() > 0)
                    {
                        foreach (var message in messages)
                        {
                            n4_r.messages.Add(new N4_response_message(message.Attribute("message-id")?.Value, message.Attribute("message-severity")?.Value, message.Attribute("message-text")?.Value));
                        }
                    }
                    n4_r.trace = String.IsNullOrEmpty(ss) ? 0 : N4Traza(ss, ouser, oclass, omethod, xmlDoc, resp);
                    return n4_r;
                }
                n4_r.status = 3;
                n4_r.messages.Add(new N4_response_message("NO_RESPONSE", "CRITICAL", "N4 RESPONSE ES NULL"));
            }
            catch (System.Xml.XmlException e)
            {
                n4_r.status = 3;
                n4_r.messages.Add(new N4_response_message("XML_ERROR", "CRITICAL", e.Message));
            }
            catch (SoapException e)
            {
                n4_r.status = 3;
                n4_r.messages.Add(new N4_response_message("SOAP_ERROR", "CRITICAL", e.Message));
            }
            catch (TimeoutException e)
            {
                n4_r.status = 3;
                n4_r.messages.Add(new N4_response_message("TIMEOUT_ERROR", "CRITICAL", e.Message));
            }
            catch (WebException e)
            {
                n4_r.status = 3;
                n4_r.messages.Add(new N4_response_message("WEB_ERROR", "CRITICAL", e.Message));
            }
            catch (Exception e)
            {
                n4_r.status = 3;
                n4_r.messages.Add(new N4_response_message("CODE_ERROR", "CRITICAL", e.Message));
            }
            //trazar todo lo que hagan en N4, usuario, clase, metodo, parametros, respuesta

            n4_r.trace = String.IsNullOrEmpty(ss) ? 0 : N4Traza(ss, ouser, oclass, omethod, xmlDoc, resp,fecha_inicio.Value,palabra_clave);
            return n4_r;
        }

        public static Int64? N4Traza(string cx, string usuario, string clase, string metodo, string entradas, string salidas)
        {
            //Grabar log completo de N4 que recibo que retorno, devolver el numero de registro formado
            if (string.IsNullOrEmpty(cx))
                return -1;

            var p = new Dictionary<string, object>();
            p.Add("origen","BRBK");
            p.Add(nameof(usuario), usuario);
            p.Add(nameof(clase), clase);
            p.Add(nameof(metodo), metodo);
            p.Add(nameof(entradas), entradas);
            p.Add(nameof(salidas), salidas);
            var r = AccesoDatos.BDOpe.ComandoInsertUpdateDeleteID(cx, "brbk.registrar_movimiento_n4", p);
            if (!r.Exitoso)
            {
                return -1;
            }
            return r.Resultado.Value;
        }

        //metodo con keyword
        public static Int64? N4Traza(string cx, string usuario, string clase, string metodo, string entradas, string salidas, DateTime fecha_inicio, string palabra_clave)
        {
            //Grabar log completo de N4 que recibo que retorno, devolver el numero de registro formado
            if (string.IsNullOrEmpty(cx))
                return -1;

            var p = new Dictionary<string, object>();
            p.Add("origen", "BRBK");
            p.Add(nameof(usuario), usuario);
            p.Add(nameof(clase), clase);
            p.Add(nameof(metodo), metodo);
            p.Add(nameof(entradas), entradas);
            p.Add(nameof(salidas), salidas);
            p.Add(nameof(fecha_inicio), fecha_inicio);
            p.Add(nameof(palabra_clave), palabra_clave);

            var r = AccesoDatos.BDOpe.ComandoInsertUpdateDeleteID(cx, "brbk.registrar_movimiento_n4_full", p);
            if (!r.Exitoso)
            {
                return -1;
            }
            return r.Resultado.Value;
        }

        public N4_BasicResponse BasicInvokeService(string xmlDoc, int timer=6000)
        {
            N4_BasicResponse n4_r = new N4_BasicResponse();
            try
            {
                basicInvoke bp = new basicInvoke();
                bp.scopeCoordinateIds = _scope;
                XDocument xds = XDocument.Parse(xmlDoc);
                bp.xmlDoc = xmlDoc;
                int tim = timer > 0 ? timer : 3000;
                var resp = this.basicInvoke(bp).basicInvokeResponse1;
                if (resp != null)
                {
                    xds = XDocument.Parse(resp);
                    //toda la respuesta de N4
                    n4_r.response = xds;
#if DEBUG
                    // string xf = "<argo-response status=\"2\" status-id=\"WARNING\"><messages><message message-id=\"INFO\" message-severity=\"INFO\" message-text=\"[id:MSCU3551446]\" message-detail=\"[id:MSCU3551446]\" /></messages><argo:snx-response xmlns:argo=\"http://www.navis.com/argo\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\"><status>OZ</status></argo:snx-response></argo-response>";
                    //    string xf = "<argo-response status=\"2\" status-id=\"WARNING\"><messages><message message-id=\"INFO\" message-severity=\"INFO\" message-text=\"[id:MSCU3551446]\" message-detail=\"[id:MSCU3551446]\" /><message message-id=\"WARNING_ISO_NOT_UPDATED\" message-severity=\"WARNING\" message-text=\"Equipment Type cannot be changed for MSCU3551446 from 22G1 to 2200 as the existing equipment type 22G1 was created at the IN_GATE which cannot be updated by the data from the SNX.\" message-detail = \"Equipment Type cannot be changed for MSCU3551446 from 22G1 to 2200 as the existing equipment type 22G1 was created at the IN_GATE which cannot be updated by the data from the SNX.\" /></messages><argo:snx-response xmlns:argo=\"http://www.navis.com/argo\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\"><status>OZ</status></argo:snx-response></argo-response>";
                    //    xds = XDocument.Parse(xf);
#endif
                    n4_r.status = (xds.Root.Attributes("status").FirstOrDefault() != null) ? int.Parse(xds.Root.Attributes("status").FirstOrDefault().Value) : -1;
                    n4_r.status_id = xds.Root.Attributes("status-id").FirstOrDefault()?.Value;

                    var snx_status = xds.Descendants().Where(u => u.Name.LocalName.Equals("status")).FirstOrDefault()?.Value;
                    if (!string.IsNullOrEmpty(snx_status))
                    {
                        n4_r.snx_response = new Snxresponse() { status = snx_status };
                    }

                    var messages = xds.Root.Descendants("messages")?.Descendants("message");
                    if (messages?.Count() > 0)
                    {
                        foreach (var message in messages)
                        {
                            n4_r.messages.Add(new N4_response_message(message.Attribute("message-id")?.Value, message.Attribute("message-severity")?.Value, message.Attribute("message-text")?.Value));
                        }
                    }
                    return n4_r;
                }
                n4_r.status = 3;
                n4_r.messages.Add(new N4_response_message("NO_RESPONSE", "CRITICAL", "N4 RESPONSE ES NULL"));
            }
            catch (System.Xml.XmlException e)
            {
                n4_r.status = 3;
                n4_r.messages.Add(new N4_response_message("XML_ERROR", "CRITICAL", e.Message));
            }
            catch (SoapException e)
            {
                n4_r.status = 3;
                n4_r.messages.Add(new N4_response_message("SOAP_ERROR", "CRITICAL", e.Message));
            }
            catch (TimeoutException e)
            {
                n4_r.status = 3;
                n4_r.messages.Add(new N4_response_message("TIMEOUT_ERROR", "CRITICAL", e.Message));
            }
            catch (WebException e)
            {
                n4_r.status = 3;
                n4_r.messages.Add(new N4_response_message("WEB_ERROR", "CRITICAL", e.Message));
            }
            catch (Exception e)
            {
                n4_r.status = 3;
                n4_r.messages.Add(new N4_response_message("CODE_ERROR", "CRITICAL", e.Message));
            }
            return n4_r;
        }
        public N4_BasicResponse BasicInvokeService<T>(T entidad, string usuario, string clase, string metodo, int timer = 6000)
        {
            var xmlDoc = SerializacionHelper.XmlSerializeEntity<T>(entidad);
            return BasicInvokeService(xmlDoc, clase, metodo, usuario, 7000);
        }
        //metodo con keyword
        public N4_BasicResponse BasicInvokeService<T>(T entidad, string usuario, string clase, string metodo, int timer = 6000, DateTime? fecha_inicio=null, string palara_clave=null)
        {
            var xmlDoc = SerializacionHelper.XmlSerializeEntity<T>(entidad);
            return BasicInvokeService(xmlDoc, clase, metodo, usuario, 7000);
        }

        public N4Ws.Entidad.N4_Bill_TransactionReponse<List<billingTransaction>> BasicInvokeService(billing bill,string clase, string metodo, string usuario, int timer = 6000)
        {
            var xmlDoc = SerializacionHelper.XmlSerializeEntity<billing>(bill);
            var n4_r = new N4Ws.Entidad.N4_Bill_TransactionReponse<List<billingTransaction>>();
            var n4= BasicInvokeService(xmlDoc, clase, metodo, usuario, 7000);
             
            n4_r.status = n4.status;
            n4_r.response = n4_r.status < 3 ? N4_Bill_TransactionReponse<billInvoice>.SerialResponse(n4.response) : null;
            n4_r.status_id = n4.status_id;
            n4_r.messages = n4.messages;
            //nuevo
            n4_r.trace = n4.trace;
            return n4_r;
        }

        //metodo con keyword
        public N4Ws.Entidad.N4_Bill_TransactionReponse<List<billingTransaction>> BasicInvokeService(billing bill, string clase, string metodo, string usuario, int timer = 6000, DateTime? fecha_inicio = null, string palara_clave = null)
        {
            var xmlDoc = SerializacionHelper.XmlSerializeEntity<billing>(bill);
            var n4_r = new N4Ws.Entidad.N4_Bill_TransactionReponse<List<billingTransaction>>();
            var n4 = BasicInvokeService(xmlDoc, clase, metodo, usuario, 7000,fecha_inicio,palara_clave);

            n4_r.status = n4.status;
            n4_r.response = n4_r.status < 3 ? N4_Bill_TransactionReponse<billInvoice>.SerialResponse(n4.response) : null;
            n4_r.status_id = n4.status_id;
            n4_r.messages = n4.messages;
            //nuevo
            n4_r.trace = n4.trace;
            return n4_r;
        }

    }

    [Serializable]
    [XmlRoot(ElementName = "N4_BasicResponse")]
    public class N4_BasicResponse
    {
        [XmlAttribute(AttributeName = "status")]
        public int status { get; set; }

        [XmlAttribute(AttributeName = "status_id")]
        public string status_id { get; set; }

        [XmlElement(ElementName =  "snx_response")]
        public Snxresponse snx_response { get; set; }

        [XmlAttribute(AttributeName = "trace-id")]
        public Int64? trace { get; set; }

        //[XmlIgnore]
        //public T xmlEntidad { get; set; }

        [XmlArray("messages")]
        [XmlArrayItem("message", typeof(N4_response_message))]
        public List<N4_response_message> messages { get; set; }

        [XmlIgnore]
        public XDocument response { get; set; }

        public N4_BasicResponse()
        {
            this.messages = new List<N4_response_message>();
        }
        public N4_BasicResponse(int _stat, string _stat_id)
        {
            this.status = _stat;
            this.status_id = _stat_id;
            this.messages = new List<N4_response_message>();
        }
    }

    [Serializable]
    [XmlRoot(ElementName = "N4_response_message")]
    public class N4_response_message
    {
        [XmlAttribute(AttributeName = "message_id")]
        public string message_id { get; set; }

        [XmlAttribute(AttributeName = "message_severity")]
        public string message_severity { get; set; }

        [XmlAttribute(AttributeName = "message_detail")]
        public string message_detail { get; set; }


        public N4_response_message(string id, string severity, string detail)
        {
            this.message_id = id;
            this.message_severity = severity;
            this.message_detail = detail;
        }
        public N4_response_message()
        {

        }
    }

    [Serializable]
    [XmlRoot(ElementName = "snx_response")]
    public class Snxresponse
    {
        [XmlAttribute(AttributeName = "snx_status")]
        public string status { get; set; }
    }
}
