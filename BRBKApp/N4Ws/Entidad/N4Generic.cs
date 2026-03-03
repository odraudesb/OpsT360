using N4Ws.N4Esquema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Web.Services.Protocols;
using System.Xml.Linq;

namespace N4Ws.Entidad
{
    internal class N4Generic:N4Esquema.ArgoService
    {
        private string _b64Credentials;
        string _username = string.Empty;
        string _password = string.Empty;
     
        protected override System.Net.WebRequest GetWebRequest(Uri uri)
        {
            System.Net.HttpWebRequest httpRequest = (System.Net.HttpWebRequest)base.GetWebRequest(uri);
            httpRequest.Headers.Add("Authorization", "Basic " + _b64Credentials);
            return (System.Net.WebRequest)httpRequest;
        }

        private N4Generic(string _user, string _password, string _url)
        {
            byte[] bCredentials = Encoding.ASCII.GetBytes(_user + ":" + _password);
            _b64Credentials = Convert.ToBase64String(bCredentials);
            this.Url = _url;
        }
        private static N4Generic instance = null;
        public static N4Generic GetInstance(string us, string pass, string url)
        {
            if (instance == null)
                instance = new N4Generic(us, pass, url);
            return instance;
        }
        public N4_GenericResponse GenericInvokeService(string xmlDoc, int timer = 6000)
        {
            N4_GenericResponse n4_r = new N4_GenericResponse();
            try
            {
                genericInvoke bp = new genericInvoke();
                bp.scopeCoordinateIdsWsType.complexId = "ECU";
                bp.scopeCoordinateIdsWsType.facilityId = "GYE";
                bp.scopeCoordinateIdsWsType.operatorId = "ICT";
                bp.scopeCoordinateIdsWsType.yardId = "CGSA";
                XDocument xds = XDocument.Parse(xmlDoc);
                bp.xmlDoc = xmlDoc;
                int tim = timer > 0 ? timer : 3000;
                var resp = this.genericInvoke(bp).genericInvokeResponse1;
                if (resp != null)
                {
                    xds = XDocument.Parse(resp.responsePayLoad);
                    //payload
                    n4_r.payload = xds;
                    n4_r.status = resp.status;
                    //messages, por si acaso
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
                n4_r.status = "ERROR";
                n4_r.messages.Add(new N4_response_message("NO_RESPONSE", "SEVERE", "N4 RESPONSE ES NULL"));
            }
            catch (System.Xml.XmlException e)
            {
                n4_r.status = "ERROR";
                n4_r.messages.Add(new N4_response_message("XML_ERROR", "CRITICAL", e.Message));
            }
            catch (SoapException e)
            {
                n4_r.status = "ERROR";
                n4_r.messages.Add(new N4_response_message("SOAP_ERROR", "CRITICAL", e.Message));
            }
            catch (TimeoutException e)
            {
                n4_r.status = "ERROR";
                n4_r.messages.Add(new N4_response_message("TIMEOUT_ERROR", "CRITICAL", e.Message));
            }
            catch (WebException e)
            {
                n4_r.status = "ERROR";
                n4_r.messages.Add(new N4_response_message("WEB_ERROR", "CRITICAL", e.Message));
            }
            catch (Exception e)
            {
                n4_r.status = "ERROR";
                n4_r.messages.Add(new N4_response_message("CODE_ERROR", "CRITICAL", e.Message));
            }
            return n4_r;
        }
        public class N4_GenericResponse
        {
            public string status { get; set; }

            public string status_id { get; set; }
            public Snxresponse snx_response { get; set; }
            public List<N4_response_message> messages { get; set; }
            public XDocument payload { get; set; }
            public N4_GenericResponse()
            {
                this.messages = new List<N4_response_message>();
            }
       
        }
    }
}
