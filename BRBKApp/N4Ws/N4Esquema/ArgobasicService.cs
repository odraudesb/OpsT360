using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace N4Ws.N4Esquema
{

    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "4.0.30319.1")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Web.Services.WebServiceBindingAttribute(Name = "ArgobasicServiceSoapBinding", Namespace = "http://www.navis.com/services/argobasicservice")]
    public abstract class ArgobasicService : System.Web.Services.Protocols.SoapHttpClientProtocol
    {

        /// <remarks/>
        public ArgobasicService()
        {
        }

        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Bare)]
        [return: System.Xml.Serialization.XmlElementAttribute("basicInvokeResponse", Namespace = "http://www.navis.com/services/argobasicservice")]
        public basicInvokeResponse basicInvoke([System.Xml.Serialization.XmlElementAttribute("basicInvoke", Namespace = "http://www.navis.com/services/argobasicservice")] basicInvoke basicInvoke1)
        {
            object[] results = this.Invoke("basicInvoke", new object[] {
                        basicInvoke1});
            return ((basicInvokeResponse)(results[0]));
        }

        /// <remarks/>
        public System.IAsyncResult BeginbasicInvoke(basicInvoke basicInvoke1, System.AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("basicInvoke", new object[] {
                        basicInvoke1}, callback, asyncState);
        }

        /// <remarks/>
        public basicInvokeResponse EndbasicInvoke(System.IAsyncResult asyncResult)
        {
            object[] results = this.EndInvoke(asyncResult);
            return ((basicInvokeResponse)(results[0]));
        }

        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Bare)]
        [return: System.Xml.Serialization.XmlElementAttribute("invokeResponse", Namespace = "http://www.navis.com/services/argobasicservice")]
        public invokeResponse invoke([System.Xml.Serialization.XmlElementAttribute(Namespace = "http://www.navis.com/services/argobasicservice")] invokeRequest invokeRequest)
        {
            object[] results = this.Invoke("invoke", new object[] {
                        invokeRequest});
            return ((invokeResponse)(results[0]));
        }

        /// <remarks/>
        public System.IAsyncResult Begininvoke(invokeRequest invokeRequest, System.AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("invoke", new object[] {
                        invokeRequest}, callback, asyncState);
        }

        /// <remarks/>
        public invokeResponse Endinvoke(System.IAsyncResult asyncResult)
        {
            object[] results = this.EndInvoke(asyncResult);
            return ((invokeResponse)(results[0]));
        }
    }

    /// <comentarios/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "4.0.30319.1")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.navis.com/services/argobasicservice")]
    public partial class basicInvoke
    {

        private string scopeCoordinateIdsField;

        private string xmlDocField;

        /// <comentarios/>
        public string scopeCoordinateIds
        {
            get
            {
                return this.scopeCoordinateIdsField;
            }
            set
            {
                this.scopeCoordinateIdsField = value;
            }
        }

        /// <comentarios/>
        public string xmlDoc
        {
            get
            {
                return this.xmlDocField;
            }
            set
            {
                this.xmlDocField = value;
            }
        }
    }

    /// <comentarios/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "4.0.30319.1")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.navis.com/services/argobasicservice")]
    public partial class basicInvokeResponse
    {

        private string basicInvokeResponse1Field;

        /// <comentarios/>
        [System.Xml.Serialization.XmlElementAttribute("basicInvokeResponse")]
        public string basicInvokeResponse1
        {
            get
            {
                return this.basicInvokeResponse1Field;
            }
            set
            {
                this.basicInvokeResponse1Field = value;
            }
        }
    }

    /// <comentarios/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "4.0.30319.1")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.navis.com/services/argobasicservice")]
    public partial class invokeRequest
    {

        private string scopeCoordinateIdsField;

        private string requestField;

        private string handlerField;

        private string localeField;

        /// <comentarios/>
        public string scopeCoordinateIds
        {
            get
            {
                return this.scopeCoordinateIdsField;
            }
            set
            {
                this.scopeCoordinateIdsField = value;
            }
        }

        /// <comentarios/>
        public string request
        {
            get
            {
                return this.requestField;
            }
            set
            {
                this.requestField = value;
            }
        }

        /// <comentarios/>
        public string handler
        {
            get
            {
                return this.handlerField;
            }
            set
            {
                this.handlerField = value;
            }
        }

        /// <comentarios/>
        public string locale
        {
            get
            {
                return this.localeField;
            }
            set
            {
                this.localeField = value;
            }
        }
    }

    /// <comentarios/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "4.0.30319.1")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.navis.com/services/argobasicservice")]
    public partial class invokeResponse
    {

        private string basicInvokeResponseField;

        /// <comentarios/>
        public string basicInvokeResponse
        {
            get
            {
                return this.basicInvokeResponseField;
            }
            set
            {
                this.basicInvokeResponseField = value;
            }
        }
    }
}


