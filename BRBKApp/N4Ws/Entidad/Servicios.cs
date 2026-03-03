using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Configuraciones;
using Respuesta;
using AccesoDatos;
using System.Reflection;
using System.Xml.Linq;

namespace N4Ws.Entidad
{
   public class Servicios : ModuloBase
    {

        public override void OnInstanceCreate()
        {
            this.alterClase = "N4_SERVICE";
            base.OnInstanceCreate();
            this.Accesorio.ConfiguracionBase = "BRBK";
        }
        //inicializa de la instancia de servicio
        private static Servicios InicializaServicio(out string erno)
        {
            var p = new Servicios();
            if (!p.Accesorio.Inicializar(out erno))
            {
                return null;
            }
            return p;
        }
        //inicializa una las configuraciones de N4
        private static N4Configuration ObtenerInicializador( Servicios ser, out string novedad)
        {
            if (ser == null)
            {
                novedad = "Objeto inicializador es nulo";
                return null;
            }
            if (!ser.Accesorio.ExistenConfiguraciones)
            {
                novedad = "No existen configuraciones de inicialización";
                return null;
            }
            var ur = ser.Accesorio.ObtenerConfiguracion("URL")?.valor;
            var us = ser.Accesorio.ObtenerConfiguracion("USUARIO")?.valor;
            var pas = ser.Accesorio.ObtenerConfiguracion("PASSWORD")?.valor;
            var sc = ser.Accesorio.ObtenerConfiguracion("SCOPE")?.valor;
           novedad = string.Empty;
          return  N4Configuration.GetInstance(us, pas, ur, sc);

        }

        public static N4_BasicResponse N4ServicioBasico<T>(T entidad,  string usuario) where T:class
        {
            //paso 1-> Inicializar instancia de servicio
            string pv;
            var p = InicializaServicio(out pv);
            var n = new N4_BasicResponse();
            p.actualMetodo = MethodBase.GetCurrentMethod().Name;
            if (p == null)
            {
                n.status = 3;
                n.status_id = "SEVERE";
                n.messages.Add(new N4_response_message("NO_INITIALIZED_DATA", "SEVERE", pv));
                return n;
            }
            //paso 2 -> inicializar instancia de n4ws
            var n4 = ObtenerInicializador(p, out pv);
            if (n4 == null)
            {
                n.status = 3;
                n.status_id = "SEVERE";
                n.messages.Add(new N4_response_message("NO_INITIALIZED_N4_INSTANCE", "SEVERE", pv));
                return n;
            }
            var nbb = N4Basic.GetInstance(n4.usuario, n4.password,n4.url, n4.scope);
            var n4r = nbb.BasicInvokeService<T>(entidad,usuario,p.myClase,p.actualMetodo, 7000);
            return n4r;
        }

        public static N4_Bill_TransactionReponse<billingTransaction> N4ServicioBasico(billing entidad,  string usuario)                                                                                                   
        {
            var ni = new N4_Bill_TransactionReponse<billingTransaction>();
            //paso 1-> Inicializar instancia de servicio
            string pv;
            var p = InicializaServicio(out pv);
        //    var n = new N4_BasicResponse();
            p.actualMetodo = MethodBase.GetCurrentMethod().Name;
            if (p == null)
            {
                ni.status = 3;
                ni.status_id = "SEVERE";
                ni.messages.Add(new N4_response_message("NO_INITIALIZED_DATA", "SEVERE", pv));
                return ni;
            }

            //paso 2 -> inicializar instancia de n4configurariones
            var n4 = ObtenerInicializador(p, out pv);
            if (n4 == null)
            {
                ni.status = 3;
                ni.status_id = "SEVERE";
                ni.messages.Add(new N4_response_message("NO_INITIALIZED_N4_INSTANCE", "SEVERE", pv));
                return ni;
            }
            var nbb = N4Basic.GetInstance(n4.usuario, n4.password, n4.url, n4.scope);

            var lbt = nbb.BasicInvokeService(entidad,p.myClase,p.actualMetodo,usuario, 7500);
            ni.status = lbt.status; ni.status_id = lbt.status_id;
            ni.messages = lbt.messages;
            ni.trace = lbt.trace;
            if (lbt.response != null)
            {
                ni.response = lbt.response.FirstOrDefault();
            }
            else { ni.response = null; }
           
            return ni;
        }

        //la entidad billing debe venir con la solicitud de finalizar
        public static N4_Bill_TransactionReponse<FinalizeInvoiceResponse> N4ServicioBasicoFinalize(billing entidad,  string usuario)
        {
            var n = new N4_Bill_TransactionReponse<FinalizeInvoiceResponse>();
            //paso 1-> Inicializar instancia de servicio
            string pv;
            var p = InicializaServicio(out pv);
            p.actualMetodo = MethodBase.GetCurrentMethod().Name;
            if (p == null)
            {
                n.status = 3;
                n.status_id = "SEVERE";
                n.messages.Add(new N4_response_message("NO_INITIALIZED_DATA", "SEVERE", pv));
                return n;
            }
            //paso 2 -> inicializar instancia de n4configurariones
            var n4 = ObtenerInicializador(p, out pv);
            if (n4 == null)
            {
                n.status = 3;
                n.status_id = "SEVERE";
                n.messages.Add(new N4_response_message("NO_INITIALIZED_N4_INSTANCE", "SEVERE", pv));
                return n;
            }
            //me quedo con el basico
            entidad.Request = null;
            if (entidad.FinalizeRequest == null)
            {
                n.status = 3;
                n.status_id = "SEVERE";
                n.messages.Add(new N4_response_message("NO_FINALIZE_REQUEST ", "SEVERE","NO SE ENCONTRO LA SOLICITUD PARA FINALIZAR"));
                return n;
            }

            var nbb = N4Basic.GetInstance(n4.usuario, n4.password, n4.url, n4.scope);
            var lbt = nbb.BasicInvokeService<billing>(entidad,usuario,p.myClase,p.actualMetodo, 7500);
            n.status = lbt.status;
            n.status_id = lbt.status_id;
            n.messages = lbt.messages;
            var xdoc = lbt.response;
            n.response = N4_Bill_TransactionReponse<billing>.FinalizeSerialResponse(lbt.response);
            return n;
        }

        public static N4_Bill_TransactionReponse<billingTransaction> N4ServicioBasicoFinalizeTransaction(billing entidad, string usuario)
        {
            var n = new N4_Bill_TransactionReponse<billingTransaction>();
            //paso 1-> Inicializar instancia de servicio
            string pv;
            var p = InicializaServicio(out pv);
            p.actualMetodo = MethodBase.GetCurrentMethod().Name;
            if (p == null)
            {
                n.status = 3;
                n.status_id = "SEVERE";
                n.trace = p.LogError<ApplicationException>(new ApplicationException("Falló Inicializador"), p.actualMetodo, usuario);
                n.messages.Add(new N4_response_message("NO_INITIALIZED_DATA", "SEVERE", pv));
                return n;
            }
            //paso 2 -> inicializar instancia de n4configurariones
            var n4 = ObtenerInicializador(p, out pv);
            if (n4 == null)
            {
                n.status = 3;
                n.status_id = "SEVERE";
                n.trace = p.LogError<ApplicationException>(new ApplicationException("Falló Inicializador"), p.actualMetodo, usuario);
                n.messages.Add(new N4_response_message("NO_INITIALIZED_N4_INSTANCE", "SEVERE", pv));
                return n;
            }


            var nbb = N4Basic.GetInstance(n4.usuario, n4.password, n4.url, n4.scope);
            var lbt = nbb.BasicInvokeService<billing>(entidad, usuario, p.myClase, p.actualMetodo, 7500);




            //RECUPERA LA FACTURA->DRAFT:INVOICE
            var inv = (from iv in lbt.response.Descendants()
                       where iv.Name.LocalName == "invoice"
                       select iv).FirstOrDefault();

            //NO ENCONTRO -> INVOICE
            if (inv == null)
            {
                n.status = 3;
                n.status_id = "SEVERE";
                n.trace = p.LogError<ApplicationException>(new ApplicationException("No se encontro INVOICE"), p.actualMetodo, usuario);
                n.messages.Add(new N4_response_message("NO_INVOICE_ITEM_ON_RESPONSE ", "SEVERE", "NO SE ENCONTRO EL ITEM INVOICE EN LA RESPUESTA DEL MERGE N4"));
                return n;
            }

            //RECUPERA EL ATRIBUTO ->NUMERO DE DRAFT
            var att = (from at in inv.Attributes()
                       where at.Name.LocalName == "draftNumber"
                       select at.Value).FirstOrDefault();
            if (att == null)
            {
                n.status = 3;
                n.status_id = "SEVERE";
                n.trace = p.LogError<ApplicationException>(new ApplicationException("No se encontro DRAFTNUMBER"), p.actualMetodo, usuario);
                n.messages.Add(new N4_response_message("NO_DRAFTNBR_ATTRIBUTE_ON_RESPONSE ", "SEVERE", "NO SE ENCONTRO EL ATRIBUTO DRAFTNBR"));
                return n;
            }
            //CONVIERTE TODO EL XML A BILLING TRASNACION
            var invo_n4 = N4_Bill_TransactionReponse<billingTransaction>.BillingTransactionData(lbt.response);
            if (invo_n4 == null)
            {
                n.status = 3;
                n.status_id = "SEVERE";
                n.trace = p.LogError<ApplicationException>(new ApplicationException("Error de conversion"), p.actualMetodo, usuario);
                n.messages.Add(new N4_response_message("CONVERSION_DATA_ERROR ", "SEVERE", "NO SE PUDO CONVERTIR XML EN BILLINGTRANSACTION"));
                return n;
            }

            if (invo_n4.billInvoice != null && !string.IsNullOrEmpty( invo_n4.billInvoice.totalTotal) && invo_n4.billInvoice.totalTotal.Equals("0.0"))
            {
                n.status = 3;
                n.status_id = "SEVERE";
                n.trace = p.LogError<ApplicationException>(new ApplicationException("INVALID_DRAFT"), p.actualMetodo, usuario);
                n.messages.Add(new N4_response_message("EMPTY_DRAFT", "SEVERE", string.Format("El Draft {0}, resulta en valor {1}, favor Borrar",invo_n4.billInvoice.draftNumber,invo_n4.billInvoice.totalTotal)));
                return n;
            }

            //PREPARA LA ENTIDAD PARA FINALIZAR
            entidad = null;
            entidad = new billing();
            entidad.FinalizeRequest = new FinalizeInvoiceRequest();
            entidad.FinalizeRequest.drftInvoiceNbr = att;
            entidad.FinalizeRequest.finalizeDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
            entidad.Request = null;

            bool finalizar_draft = false;
            //si existen configuraciones
            if (p.Accesorio.ExistenConfiguraciones)
            {
              var cc = p.Accesorio.ObtenerConfiguracion("FINALIZAR");
                if (cc != null && cc.valor.Equals("SI"))
                {
                    finalizar_draft = true;
                }
            }
            if (finalizar_draft == false)
            {
                n.status = 0;
                n.status_id = "OK";
                n.messages = new List<N4_response_message>();
                invo_n4.billInvoice.finalNumber = "00000000000001";
                n.response = invo_n4;
                n.trace = -1;
                return n;
            }

            else
            {
                //SECUENCIA DE FINALIZACION
                lbt = nbb.BasicInvokeService<billing>(entidad, usuario, p.myClase, p.actualMetodo, 7500);
                n.status = lbt.status;
                n.status_id = lbt.status_id;
                n.messages = lbt.messages;
                var xdoc = lbt.response;
                invo_n4.billInvoice.finalNumber = N4_Bill_TransactionReponse<billingTransaction>.FinalizarNumeroFinal(lbt.response);
                n.response = invo_n4;
                n.trace = lbt.trace;
                return n;
            }
        }
        private static N4_BasicResponse N4ICUService(ICU_API icu, string usuario)
        {
            var n = new N4_BasicResponse();
            string pv;
            var p = InicializaServicio(out pv);
            p.actualMetodo = MethodBase.GetCurrentMethod().Name;
            if (p == null)
            {
                n.status = 3;
                n.status_id = "SEVERE";
                n.messages.Add(new N4_response_message("NO_INITIALIZED_DATA", "SEVERE", pv));
                return n;
            }
            //paso 2 -> inicializar instancia de n4configurariones
            var n4 = ObtenerInicializador(p, out pv);
            if (n4 == null)
            {
                n.status = 3;
                n.status_id = "SEVERE";
                n.messages.Add(new N4_response_message("NO_INITIALIZED_N4_INSTANCE", "SEVERE", pv));
                return n;
            }
            if (icu == null)
            {
                n.status = 3;
                n.status_id = "SEVERE";
                n.messages.Add(new N4_response_message("NO_ICU_INSTANCE", "SEVERE", "NO SE ENCONTRO OBJETO ICU DE API"));
                return n;
            }
            var nbb = N4Basic.GetInstance(n4.usuario, n4.password, n4.url, n4.scope);



            return nbb.BasicInvokeService<ICU_API>(icu, usuario, p.myClase, p.actualMetodo, 7500);

        }


        public static N4_Bill_TransactionReponse<FinalizeInvoiceResponse> N4ServicioBasicoMergeAndFinalize(billing entidad,  string usuario)
        {
 
            var n = new N4_Bill_TransactionReponse<FinalizeInvoiceResponse>();
            //paso 1-> Inicializar instancia de servicio
            string pv;
            var p = InicializaServicio(out pv);
            p.actualMetodo = MethodBase.GetCurrentMethod().Name;
            if (p == null)
            {
                n.status = 3;
                n.status_id = "SEVERE";
                n.messages.Add(new N4_response_message("NO_INITIALIZED_DATA", "SEVERE", pv));
                return n;
            }
            //paso 2 -> inicializar instancia de n4configurariones
            var n4 = ObtenerInicializador(p, out pv);
            if (n4 == null)
            {
                n.status = 3;
                n.status_id = "SEVERE";
                n.messages.Add(new N4_response_message("NO_INITIALIZED_N4_INSTANCE", "SEVERE", pv));
                return n;
            }

           /*
            if (string.IsNullOrEmpty(entidad.Request.InvoiceTypeId))
            {
                n.status = 3;
                n.status_id = "SEVERE";
                n.messages.Add(new N4_response_message("NO_INVOICE_TYPE_ID", "SEVERE", "NO SE ENOCNTRÓ EL NOMBRE DEL INVOICE TYPE EN EL ELEMENTO BILLING REQUEST"));
                return n;
            }
            */
            //aqui comprobar que la lista de draftVenga

            if (entidad.MergeInvoiceRequest == null || entidad.MergeInvoiceRequest.drftInvoiceNbrs.Count < 0)
            {
                n.status = 3;
                n.status_id = "SEVERE";
                n.messages.Add(new N4_response_message("NO_DRAFT_NBRS", "SEVERE", "NO SE ENCONTRO LOS NUMEROS DE DRAFT"));
                return n;
            }

            var it = entidad.Request != null ? entidad.Request.InvoiceTypeId : null;
            if (entidad.MergeInvoiceRequest == null)
            {
                n.status = 3;
                n.status_id = "SEVERE";
                n.messages.Add(new N4_response_message("NO_MERGE_REQUEST ", "SEVERE", "NO SE ENCONTRO LA SOLICITUD PARA MERGE"));
                return n;
            }

           // entidad.MergeInvoiceRequest.invoiceTypeId = entidad.Request.InvoiceTypeId;
            entidad.Request = null;
            entidad.FinalizeRequest = null;
            if (string.IsNullOrEmpty(it))
            {
                it = entidad.MergeInvoiceRequest.invoiceTypeId != null ? entidad.MergeInvoiceRequest.invoiceTypeId : null;
            }
            else
            {
                entidad.MergeInvoiceRequest.invoiceTypeId = entidad.MergeInvoiceRequest.invoiceTypeId == null ? it : null;
            }

            if (string.IsNullOrEmpty(it))
            {
                n.status = 3;
                n.status_id = "SEVERE";
                n.messages.Add(new N4_response_message("NO_INVOICE_TYPE_IN_MERGE_OR_BILLING_REQUEST ", "SEVERE", "NO SE ENCONTRO EL INVOICETYPEID, NI EN BILLING REQUEST O MERGE REQUEST"));
                return n;
            }
            var nbb = N4Basic.GetInstance(n4.usuario, n4.password, n4.url, n4.scope);
            string findate = entidad.MergeInvoiceRequest.finalizeDate;
            entidad.MergeInvoiceRequest.finalizeDate = null;

            //manda el merge
            var lbt = nbb.BasicInvokeService<billing>(entidad, usuario, p.myClase, p.actualMetodo, 7500);
            //EL MERGE ME REGresa una factura completa el documento, solo entreguemos el numero final o la factura?

            //hubo un error en el merge
            if (lbt.status > 2)
            {
                n.status = lbt.status;
                n.status_id = string.Format("MERGE_ERROR:{0}",lbt.status_id);
                n.messages = lbt.messages;
                return n;
            }
            //RECUPERA LA FACTURA->DRAFT
            var inv = (from iv in lbt.response.Descendants()
                       where iv.Name.LocalName == "invoice"
                       select iv).FirstOrDefault();


            if (inv == null)
            {
                n.status = 3;
                n.status_id = "SEVERE";
                n.messages.Add(new N4_response_message("NO_INVOICE_ITEM_ON_RESPONSE ", "SEVERE", "NO SE ENCONTRO EL ITEM INVOICE EN LA RESPUESTA DEL MERGE N4"));
                return n;
            }

            //RECUPERA EL ATRIBUTO ->NUMERO
            var att = (from at in inv.Attributes()
                       where at.Name.LocalName == "draftNumber"
                       select at.Value).FirstOrDefault();
            if (att == null)
            {
                n.status = 3;
                n.status_id = "SEVERE";
                n.messages.Add(new N4_response_message("NO_DRAFTNBR_ATTRIBUTE_ON_RESPONSE ", "SEVERE", "NO SE ENCONTRO EL ATRIBUTO DRAFTNBR"));
                return n;
            }
            
            entidad = null;
            entidad = new billing();
            entidad.FinalizeRequest = new FinalizeInvoiceRequest();
            entidad.FinalizeRequest.drftInvoiceNbr = att;
            entidad.FinalizeRequest.finalizeDate = findate;
            entidad.Request = null;

            //ahora invoca la finalizacion
            lbt = nbb.BasicInvokeService<billing>(entidad, usuario, p.myClase, p.actualMetodo, 7500);
            n.status = lbt.status;
            n.status_id = lbt.status_id;
            n.messages = lbt.messages;
            var xdoc = lbt.response;
            n.response = N4_Bill_TransactionReponse<billing>.FinalizeSerialResponse(lbt.response);
            return n;
        }



        //REVISAR
        public static N4_Bill_TransactionReponse<billingTransaction> N4ServicioBasicoMergeAndFinalizeTransaction(billing entidad, string usuario)
        {

            var n = new N4_Bill_TransactionReponse<billingTransaction>();
            //paso 1-> Inicializar instancia de servicio
            string pv;
            var p = InicializaServicio(out pv);
            p.actualMetodo = MethodBase.GetCurrentMethod().Name;
            if (p == null)
            {
                n.status = 3;
                n.status_id = "SEVERE";
                n.messages.Add(new N4_response_message("NO_INITIALIZED_DATA", "SEVERE", pv));
                n.trace = p.LogError<ApplicationException>(new ApplicationException("Falló Inicializador"), p.actualMetodo, usuario);
                return n;
            }
            //paso 2 -> inicializar instancia de n4configurariones
            var n4 = ObtenerInicializador(p, out pv);
            if (n4 == null)
            {
                n.status = 3;
                n.status_id = "SEVERE";
                n.trace = p.LogError<ApplicationException>(new ApplicationException("Falló Inicializador"), p.actualMetodo, usuario);
              
                n.messages.Add(new N4_response_message("NO_INITIALIZED_N4_INSTANCE", "SEVERE", pv));
                return n;
            }

            /*
             if (string.IsNullOrEmpty(entidad.Request.InvoiceTypeId))
             {
                 n.status = 3;
                 n.status_id = "SEVERE";
                 n.messages.Add(new N4_response_message("NO_INVOICE_TYPE_ID", "SEVERE", "NO SE ENOCNTRÓ EL NOMBRE DEL INVOICE TYPE EN EL ELEMENTO BILLING REQUEST"));
                 return n;
             }
             */
            //aqui comprobar que la lista de draftVenga

            if (entidad.MergeInvoiceRequest == null || entidad.MergeInvoiceRequest.drftInvoiceNbrs.Count < 0)
            {
                n.status = 3;
                n.status_id = "SEVERE";
                n.trace = p.LogError<ApplicationException>(new ApplicationException("No hay numeros de DRAFT"), p.actualMetodo, usuario);
                n.messages.Add(new N4_response_message("NO_DRAFT_NBRS", "SEVERE", "NO SE ENCONTRO LOS NUMEROS DE DRAFT"));
                return n;
            }

            var it = entidad.Request != null ? entidad.Request.InvoiceTypeId : null;
            if (entidad.MergeInvoiceRequest == null)
            {
                n.status = 3;
                n.status_id = "SEVERE";
                n.trace = p.LogError<ApplicationException>(new ApplicationException("SIN SOLICITUD DE MERGE"), p.actualMetodo, usuario);
                n.messages.Add(new N4_response_message("NO_MERGE_REQUEST ", "SEVERE", "NO SE ENCONTRO LA SOLICITUD PARA MERGE"));
                return n;
            }

        //    entidad.MergeInvoiceRequest.invoiceTypeId = entidad.Request.InvoiceTypeId;
            entidad.Request = null;
            entidad.FinalizeRequest = null;
            if (string.IsNullOrEmpty(it))
            {
                it = entidad.MergeInvoiceRequest.invoiceTypeId != null ? entidad.MergeInvoiceRequest.invoiceTypeId : null;
            }
            else
            {
                entidad.MergeInvoiceRequest.invoiceTypeId = entidad.MergeInvoiceRequest.invoiceTypeId == null ? it : null;
            }

            if (string.IsNullOrEmpty(it))
            {
                n.status = 3;
                n.status_id = "SEVERE";
                n.trace = p.LogError<ApplicationException>(new ApplicationException("SIN INVOICETYPEID"), p.actualMetodo, usuario);
                n.messages.Add(new N4_response_message("NO_INVOICE_TYPE_IN_MERGE_OR_BILLING_REQUEST ", "SEVERE", "NO SE ENCONTRO EL INVOICETYPEID, NI EN BILLING REQUEST O MERGE REQUEST"));
                return n;
            }
            var nbb = N4Basic.GetInstance(n4.usuario, n4.password, n4.url, n4.scope);
            string findate = entidad.MergeInvoiceRequest.finalizeDate;
            entidad.MergeInvoiceRequest.finalizeDate = null;

            //manda el merge
            var lbt = nbb.BasicInvokeService<billing>(entidad, usuario, p.myClase, p.actualMetodo, 7500);
            //EL MERGE ME REGresa una factura completa el documento, solo entreguemos el numero final o la factura?

            //hubo un error en el merge
            if (lbt.status > 2)
            {
                n.status = lbt.status;
                n.trace = p.LogError<ApplicationException>(new ApplicationException(lbt.status_id), p.actualMetodo, usuario);
                n.status_id = string.Format("MERGE_ERROR:{0}", lbt.status_id);
                n.trace = lbt.trace;
                n.messages = lbt.messages;
                return n;
            }
            //RECUPERA LA FACTURA->DRAFT:INVOICE
            var inv = (from iv in lbt.response.Descendants()
                       where iv.Name.LocalName == "invoice"
                       select iv).FirstOrDefault();

            //NO ENCONTRO -> INVOICE
            if (inv == null)
            {
                n.status = 3;
                n.status_id = "SEVERE";
                n.trace = p.LogError<ApplicationException>(new ApplicationException("No se encontró elemento INVOICE"), p.actualMetodo, usuario);
                n.messages.Add(new N4_response_message("NO_INVOICE_ITEM_ON_RESPONSE ", "SEVERE", "NO SE ENCONTRO EL ITEM INVOICE EN LA RESPUESTA DEL MERGE N4"));
                return n;
            }

            //RECUPERA EL ATRIBUTO ->NUMERO DE DRAFT
            var att = (from at in inv.Attributes()
                       where at.Name.LocalName == "draftNumber"
                       select at.Value).FirstOrDefault();
            if (att == null)
            {
                n.status = 3;
                n.status_id = "SEVERE";
                n.trace = p.LogError<ApplicationException>(new ApplicationException("No se encontro DRAFTNUMBER"), p.actualMetodo, usuario);
                n.messages.Add(new N4_response_message("NO_DRAFTNBR_ATTRIBUTE_ON_RESPONSE ", "SEVERE", "NO SE ENCONTRO EL ATRIBUTO DRAFTNBR"));
                return n;
            }
            //CONVIERTE TODO EL XML A BILLING TRASNACION
            var invo_n4 = N4_Bill_TransactionReponse<billingTransaction>.BillingTransactionData(lbt.response);
            if (invo_n4 == null)
            {
                n.status = 3;
                n.status_id = "SEVERE";
                n.trace = p.LogError<ApplicationException>(new ApplicationException("ERROR DE CONVERSION"), p.actualMetodo, usuario);
                n.messages.Add(new N4_response_message("CONVERSION_DATA_ERROR ", "SEVERE", "NO SE PUDO CONVERTIR XML EN BILLINGTRANSACTION"));
                return n;
            }
            //PREPARA LA ENTIDAD PARA FINALIZAR
            entidad = null;
            entidad = new billing();
            entidad.FinalizeRequest = new FinalizeInvoiceRequest();
            entidad.FinalizeRequest.drftInvoiceNbr = att;
            entidad.FinalizeRequest.finalizeDate = findate;
            entidad.Request = null;

            //ahora invoca la finalizacion
            lbt = nbb.BasicInvokeService<billing>(entidad, usuario, p.myClase, p.actualMetodo, 7500);
            //AQUI YA FINALIZÓ
            n.status = lbt.status;
            n.status_id = lbt.status_id;
            n.messages = lbt.messages;
            var xdoc = lbt.response;
            //RECUPERO EL NUMERO FINAL Y LO PONGO AL INVOICE
            invo_n4.billInvoice.finalNumber = N4_Bill_TransactionReponse<billingTransaction>.FinalizarNumeroFinal(lbt.response);
            n.response = invo_n4;
            return n;
        }

        //con keyword
        public static N4_Bill_TransactionReponse<billingTransaction> N4ServicioBasicoMergeAndFinalizeTransaction(billing entidad, string usuario, DateTime? fecha_inicio=null, string palabra_clave=null)
        {

            var n = new N4_Bill_TransactionReponse<billingTransaction>();
            //paso 1-> Inicializar instancia de servicio
            string pv;
            var p = InicializaServicio(out pv);
            p.actualMetodo = MethodBase.GetCurrentMethod().Name;
            if (p == null)
            {
                n.status = 3;
                n.status_id = "SEVERE";
                n.messages.Add(new N4_response_message("NO_INITIALIZED_DATA", "SEVERE", pv));
                n.trace = p.LogError<ApplicationException>(new ApplicationException("Falló Inicializador"), p.actualMetodo, usuario);
                return n;
            }
            //paso 2 -> inicializar instancia de n4configurariones
            var n4 = ObtenerInicializador(p, out pv);
            if (n4 == null)
            {
                n.status = 3;
                n.status_id = "SEVERE";
                n.trace = p.LogError<ApplicationException>(new ApplicationException("Falló Inicializador"), p.actualMetodo, usuario);

                n.messages.Add(new N4_response_message("NO_INITIALIZED_N4_INSTANCE", "SEVERE", pv));
                return n;
            }

            /*
             if (string.IsNullOrEmpty(entidad.Request.InvoiceTypeId))
             {
                 n.status = 3;
                 n.status_id = "SEVERE";
                 n.messages.Add(new N4_response_message("NO_INVOICE_TYPE_ID", "SEVERE", "NO SE ENOCNTRÓ EL NOMBRE DEL INVOICE TYPE EN EL ELEMENTO BILLING REQUEST"));
                 return n;
             }
             */
            //aqui comprobar que la lista de draftVenga

            if (entidad.MergeInvoiceRequest == null || entidad.MergeInvoiceRequest.drftInvoiceNbrs.Count < 0)
            {
                n.status = 3;
                n.status_id = "SEVERE";
                n.trace = p.LogError<ApplicationException>(new ApplicationException("No hay numeros de DRAFT"), p.actualMetodo, usuario);
                n.messages.Add(new N4_response_message("NO_DRAFT_NBRS", "SEVERE", "NO SE ENCONTRO LOS NUMEROS DE DRAFT"));
                return n;
            }

            var it = entidad.Request != null ? entidad.Request.InvoiceTypeId : null;
            if (entidad.MergeInvoiceRequest == null)
            {
                n.status = 3;
                n.status_id = "SEVERE";
                n.trace = p.LogError<ApplicationException>(new ApplicationException("SIN SOLICITUD DE MERGE"), p.actualMetodo, usuario);
                n.messages.Add(new N4_response_message("NO_MERGE_REQUEST ", "SEVERE", "NO SE ENCONTRO LA SOLICITUD PARA MERGE"));
                return n;
            }

            //    entidad.MergeInvoiceRequest.invoiceTypeId = entidad.Request.InvoiceTypeId;
            entidad.Request = null;
            entidad.FinalizeRequest = null;
            if (string.IsNullOrEmpty(it))
            {
                it = entidad.MergeInvoiceRequest.invoiceTypeId != null ? entidad.MergeInvoiceRequest.invoiceTypeId : null;
            }
            else
            {
                entidad.MergeInvoiceRequest.invoiceTypeId = entidad.MergeInvoiceRequest.invoiceTypeId == null ? it : null;
            }

            if (string.IsNullOrEmpty(it))
            {
                n.status = 3;
                n.status_id = "SEVERE";
                n.trace = p.LogError<ApplicationException>(new ApplicationException("SIN INVOICETYPEID"), p.actualMetodo, usuario);
                n.messages.Add(new N4_response_message("NO_INVOICE_TYPE_IN_MERGE_OR_BILLING_REQUEST ", "SEVERE", "NO SE ENCONTRO EL INVOICETYPEID, NI EN BILLING REQUEST O MERGE REQUEST"));
                return n;
            }
            var nbb = N4Basic.GetInstance(n4.usuario, n4.password, n4.url, n4.scope);
            string findate = entidad.MergeInvoiceRequest.finalizeDate;
            entidad.MergeInvoiceRequest.finalizeDate = null;

            //manda el merge
            var lbt = nbb.BasicInvokeService<billing>(entidad, usuario, p.myClase, p.actualMetodo, 7500,fecha_inicio,palabra_clave);
            //EL MERGE ME REGresa una factura completa el documento, solo entreguemos el numero final o la factura?

            //hubo un error en el merge
            if (lbt.status > 2)
            {
                n.status = lbt.status;
                n.trace = p.LogError<ApplicationException>(new ApplicationException(lbt.status_id), p.actualMetodo, usuario);
                n.status_id = string.Format("MERGE_ERROR:{0}", lbt.status_id);
                n.trace = lbt.trace;
                n.messages = lbt.messages;
                return n;
            }
            //RECUPERA LA FACTURA->DRAFT:INVOICE
            var inv = (from iv in lbt.response.Descendants()
                       where iv.Name.LocalName == "invoice"
                       select iv).FirstOrDefault();

            //NO ENCONTRO -> INVOICE
            if (inv == null)
            {
                n.status = 3;
                n.status_id = "SEVERE";
                n.trace = p.LogError<ApplicationException>(new ApplicationException("No se encontró elemento INVOICE"), p.actualMetodo, usuario);
                n.messages.Add(new N4_response_message("NO_INVOICE_ITEM_ON_RESPONSE ", "SEVERE", "NO SE ENCONTRO EL ITEM INVOICE EN LA RESPUESTA DEL MERGE N4"));
                return n;
            }

            //RECUPERA EL ATRIBUTO ->NUMERO DE DRAFT
            var att = (from at in inv.Attributes()
                       where at.Name.LocalName == "draftNumber"
                       select at.Value).FirstOrDefault();
            if (att == null)
            {
                n.status = 3;
                n.status_id = "SEVERE";
                n.trace = p.LogError<ApplicationException>(new ApplicationException("No se encontro DRAFTNUMBER"), p.actualMetodo, usuario);
                n.messages.Add(new N4_response_message("NO_DRAFTNBR_ATTRIBUTE_ON_RESPONSE ", "SEVERE", "NO SE ENCONTRO EL ATRIBUTO DRAFTNBR"));
                return n;
            }
            //CONVIERTE TODO EL XML A BILLING TRASNACION
            var invo_n4 = N4_Bill_TransactionReponse<billingTransaction>.BillingTransactionData(lbt.response);
            if (invo_n4 == null)
            {
                n.status = 3;
                n.status_id = "SEVERE";
                n.trace = p.LogError<ApplicationException>(new ApplicationException("ERROR DE CONVERSION"), p.actualMetodo, usuario);
                n.messages.Add(new N4_response_message("CONVERSION_DATA_ERROR ", "SEVERE", "NO SE PUDO CONVERTIR XML EN BILLINGTRANSACTION"));
                return n;
            }
            //PREPARA LA ENTIDAD PARA FINALIZAR
            entidad = null;
            entidad = new billing();
            entidad.FinalizeRequest = new FinalizeInvoiceRequest();
            entidad.FinalizeRequest.drftInvoiceNbr = att;
            entidad.FinalizeRequest.finalizeDate = findate;
            entidad.Request = null;

            //ahora invoca la finalizacion
            lbt = nbb.BasicInvokeService<billing>(entidad, usuario, p.myClase, p.actualMetodo, 7500);
            //AQUI YA FINALIZÓ
            n.status = lbt.status;
            n.status_id = lbt.status_id;
            n.messages = lbt.messages;
            var xdoc = lbt.response;
            //RECUPERO EL NUMERO FINAL Y LO PONGO AL INVOICE
            invo_n4.billInvoice.finalNumber = N4_Bill_TransactionReponse<billingTransaction>.FinalizarNumeroFinal(lbt.response);
            n.response = invo_n4;
            return n;
        }


        public static N4_BasicResponse CrearNuevoAppointment(gate gt, string usuario)
        {
            var n = new N4_BasicResponse();
            //paso 1-> Inicializar instancia de servicio
            string pv;
            var p = InicializaServicio(out pv);
            p.actualMetodo = MethodBase.GetCurrentMethod().Name;
            if (p == null)
            {
                n.status = 3;
                n.status_id = "SEVERE";
                n.messages.Add(new N4_response_message("NO_INITIALIZED_DATA", "SEVERE", pv));
                return n;
            }
            //paso 2 -> inicializar instancia de n4configurariones
            var n4 = ObtenerInicializador(p, out pv);
            if (n4 == null)
            {
                n.status = 3;
                n.status_id = "SEVERE";
                n.messages.Add(new N4_response_message("NO_INITIALIZED_N4_INSTANCE", "SEVERE", pv));
                return n;
            }
            if (gt== null || gt.appointment== null)
            {
                n.status = 3;
                n.status_id = "SEVERE";
                n.messages.Add(new N4_response_message("NO_APPOINTMET_FOR_GATE", "SEVERE", "gate es nulo"));
                return n;

            }
            if (!gt.appointment.appointment_date.HasValue)
            {
                n.status = 3;
                n.status_id = "SEVERE";
                n.messages.Add(new N4_response_message("NO_DATE_FOR_APPOINTMENT", "SEVERE", "NO ha fecha de appointment"));
                return n;

            }
            if (string.IsNullOrEmpty( gt.appointment.container_id))
            {
                n.status = 3;
                n.status_id = "SEVERE";
                n.messages.Add(new N4_response_message("NO_EQUIP_FOR_APPOINTMENT", "SEVERE", "NO ha fecha de appointment"));
                return n;
            }
            var nbb = N4Basic.GetInstance(n4.usuario, n4.password, n4.url, n4.scope);
            var gs = gt.ToString();

           var aprr = nbb.BasicInvokeService(gs, p.myClase, p.actualMetodo, usuario, 7000);

            var fir = aprr.response.Descendants().Where(nf => nf.Name.LocalName== "appointment-nbr").FirstOrDefault();
            if (fir != null)
            {
                aprr.messages.Add(new N4_response_message("appointment", fir.Value, "appointment-nbr"));
            }

            return aprr ;
        }

        public static N4_BasicResponse CancelarAppointments(gate gt, string usuario)
        {
            var n = new N4_BasicResponse();
            //paso 1-> Inicializar instancia de servicio
            string pv;
            var p = InicializaServicio(out pv);
            p.actualMetodo = MethodBase.GetCurrentMethod().Name;
            if (p == null)
            {
                n.status = 3;
                n.status_id = "SEVERE";
                n.messages.Add(new N4_response_message("NO_INITIALIZED_DATA", "SEVERE", pv));
                return n;
            }
            //paso 2 -> inicializar instancia de n4configurariones
            var n4 = ObtenerInicializador(p, out pv);
            if (n4 == null)
            {
                n.status = 3;
                n.status_id = "SEVERE";
                n.messages.Add(new N4_response_message("NO_INITIALIZED_N4_INSTANCE", "SEVERE", pv));
                return n;
            }

            if (gt.appointments== null || gt.appointments.Count<=0)
            {
                n.status = 3;
                n.status_id = "SEVERE";
                n.messages.Add(new N4_response_message("NO_APPOINTMENTS_FOR_CANCEL","SEVERE","No existen appointments que cancelar"));
                return n;

            }
            var nbb = N4Basic.GetInstance(n4.usuario, n4.password, n4.url, n4.scope);
            var gs = gt.ToString();
            return nbb.BasicInvokeService(gs,p.myClase,p.actualMetodo,usuario, 7000);
        }

        private static N4_BasicResponse ReeferHours(CGSAComputeAndSplitReeferHoursWS rh, string usuario)
        {
            var n = new N4_BasicResponse();
            if (rh == null)
            {
                n.status = 3;
                n.status_id = "SEVERE";
                n.messages.Add(new N4_response_message("NO_PARAMETERS", "SEVERE", "Entidad es nula RH"));
               return n;

            }
            //paso 1-> Inicializar instancia de servicio
            string pv;
            var p = InicializaServicio(out pv);
            p.actualMetodo = MethodBase.GetCurrentMethod().Name;
            if (p == null)
            {
                n.status = 3;
                n.status_id = "SEVERE";
                n.messages.Add(new N4_response_message("NO_INITIALIZED_DATA", "SEVERE", pv));
                return n;
            }
            //paso 2 -> inicializar instancia de n4configurariones
            var n4 = ObtenerInicializador(p, out pv);
            if (n4 == null)
            {
                n.status = 3;
                n.status_id = "SEVERE";
                n.messages.Add(new N4_response_message("NO_INITIALIZED_N4_INSTANCE", "SEVERE", pv));
                return n;
            }

            var nbb = N4Basic.GetInstance(n4.usuario, n4.password, n4.url, n4.scope);
            var gs = rh.ToString();
            return nbb.BasicInvokeService(gs,p.myClase,p.actualMetodo,usuario, 7000);
        }

        
        //retorna nulo es error!!!
        public static void ReeferImpoHour( List<Int64> gkeys, string reference, DateTime cutDay, string usuario)
        {
            var s = new Servicios();
            string pv;
            var p = InicializaServicio(out pv);
            p.actualMetodo = MethodBase.GetCurrentMethod().Name;
            if (p == null)
            {
                p.LogError<ApplicationException>(new ApplicationException("No fue posible inicializar objeto servicios"), p.actualMetodo, usuario);
                //trace log not init
            }
            var dd = new Dictionary<string, string>();
            if (gkeys.Count <= 0)
            {
                //save log no count
                p.LogError<ApplicationException>(new ApplicationException("La lista de gkey es vacia"), p.actualMetodo, usuario);
            }
            foreach (var g in gkeys)
            {
                var cg = new CGSAComputeAndSplitReeferHoursWS(g.ToString(), cutDay.ToString("yyyy-MM-dd HH:mm:ss"), reference);
                var n4r = ReeferHours(cg, usuario);
                if (n4r.status > 2)
                {
                    //log problem
                    p.LogError<ApplicationException>(new ApplicationException (string.Format("{0},{1}",g, n4r.status_id)), p.actualMetodo, usuario);
                }
            }
        }


        public static Respuesta.ResultadoOperacion<bool> PonerEventoPasePuerta(string contenedor, string usuario)
        {
            string pv;
            var p = InicializaServicio(out pv);
            p.actualMetodo = MethodBase.GetCurrentMethod().Name;
            if (p == null)
            {
                p.LogError<ApplicationException>(new ApplicationException("No fue posible inicializar objeto servicios"), p.actualMetodo, usuario);
                //trace log not init
                return Respuesta.ResultadoOperacion<bool>.CrearFalla("La inicialización del objeto N4 falló");
            }

            if (!p.Accesorio.ExistenConfiguraciones)
            {
                return Respuesta.ResultadoOperacion<bool>.CrearFalla("No existen configuraciones");
            }

            var evento = p.Accesorio.ObtenerConfiguracion("PASE_VENCIDO")?.valor;
            var notas = p.Accesorio.ObtenerConfiguracion("NOTA_EVENTO")?.valor;
            var propiedad = p.Accesorio.ObtenerConfiguracion("PROPIEDAD")?.valor;
            var tipo = p.Accesorio.ObtenerConfiguracion("TIPO")?.valor;

            var icu = new ICU_API();
            icu.evento = new ICU_EVENT();
            icu.evento.id = evento;
            icu.evento.timeeventapplied = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
            icu.evento.userid = usuario;
            icu.evento.note = notas;
            


            icu.units = new List<ICU_UNIT>();
            icu.units.Add(new ICU_UNIT() { id = contenedor, type = tipo });

            icu.properties = new List<ICU_PROPERTY>();
            icu.properties.Add(new ICU_PROPERTY() { tag=propiedad, value= notas});

            var tr = N4ICUService(icu, usuario);
            if (tr.status > 2)
            {
                return Respuesta.ResultadoOperacion<bool>.CrearFalla(tr.status_id);
            }


            return Respuesta.ResultadoOperacion<bool>.CrearResultadoExitoso(true,   "Éxito al aplicar el evento");
        }


        public static N4_BasicResponse EjecutarCODEExtension(GroovyCodeExtension co, string usuario)
        {
            var n = new N4_BasicResponse();
            //paso 1-> Inicializar instancia de servicio
            string pv;
            var p = InicializaServicio(out pv);
            p.actualMetodo = MethodBase.GetCurrentMethod().Name;
            if (p == null)
            {
                n.status = 3;
                n.status_id = "SEVERE";
                n.messages.Add(new N4_response_message("NO_INITIALIZED_DATA", "SEVERE", pv));
                return n;
            }
            //paso 2 -> inicializar instancia de n4configurariones
            var n4 = ObtenerInicializador(p, out pv);
            if (n4 == null)
            {
                n.status = 3;
                n.status_id = "SEVERE";
                n.messages.Add(new N4_response_message("NO_INITIALIZED_N4_INSTANCE", "SEVERE", pv));
                return n;
            }
            if (co == null)
            {
                n.status = 3;
                n.status_id = "SEVERE";
                n.messages.Add(new N4_response_message("NO_CODE_EXENSION_NULL", "SEVERE", "CODE_EXTENSON es nulo"));
                return n;

            }
            if (string.IsNullOrEmpty(co.location))
            {
                n.status = 3;
                n.status_id = "SEVERE";
                n.messages.Add(new N4_response_message("NO_LOCATION_FOR_CODE_EXTENSION", "SEVERE", "NO HAY LOCACION"));
                return n;

            }
            if (string.IsNullOrEmpty(co.name))
            {
                n.status = 3;
                n.status_id = "SEVERE";
                n.messages.Add(new N4_response_message("NO_NAME_FOR_CODE_EXTENSION", "SEVERE", "NO HAY NOMBRE"));
                return n;
            }
            var nbb = N4Basic.GetInstance(n4.usuario, n4.password, n4.url, n4.scope);
            var gs = co.ToString();

            var aprr = nbb.BasicInvokeService(gs, p.myClase, p.actualMetodo, usuario, 7000);

            //obtener el manifiesto
            var bil = co.parameters.Where(s => s.Key.Equals("BLs")).FirstOrDefault();

            var fir = aprr.response.Descendants().Where(nf => nf.Name.LocalName == "result").FirstOrDefault();
            if (fir != null && !string.IsNullOrEmpty(fir.Value))
            {
                Int32 numero;
                aprr.messages.Clear();
                if (fir.Value.Contains("OK-") || int.TryParse(fir.Value,out numero))
                {
                    fir.Value = fir.Value.Replace("OK-", "");
                    aprr.messages.Add(new N4_response_message("pase_id", fir.Value, "result"));
                }
                else
                {
                    if (fir.Value.Contains("ERRKEY_QTY_ORDERED_EXCEEDS_AVAILABLE_QTY_FOR_CARGO_LOT") || fir.Value.Contains("ERRKEY_QTY_ORDERED_EXCEEDS_AVAILABLE_QTY_FOR_BL_ITEM"))
                    {
                        aprr.status = 3;
                        aprr.status_id = "ITEMS_QTY_EXCEDED";
                        aprr.messages.Add(new N4_response_message("STOCK EXCEDEED", "SEVERE", string.Format("Carga {0} no tiene stock disponible", bil.Value)));
                    }
                }
            }
            return aprr;
        }


        public static N4_BasicResponse EjecutarCODEExtensionGenerico(GroovyCodeExtension co, string usuario)
        {
            var n = new N4_BasicResponse();
            //paso 1-> Inicializar instancia de servicio
            string pv;
            var p = InicializaServicio(out pv);
            p.actualMetodo = MethodBase.GetCurrentMethod().Name;
            if (p == null)
            {
                n.status = 3;
                n.status_id = "SEVERE";
                n.messages.Add(new N4_response_message("NO_INITIALIZED_DATA", "SEVERE", pv));
                return n;
            }
            //paso 2 -> inicializar instancia de n4configurariones
            var n4 = ObtenerInicializador(p, out pv);
            if (n4 == null)
            {
                n.status = 3;
                n.status_id = "SEVERE";
                n.messages.Add(new N4_response_message("NO_INITIALIZED_N4_INSTANCE", "SEVERE", pv));
                return n;
            }
            if (co == null)
            {
                n.status = 3;
                n.status_id = "SEVERE";
                n.messages.Add(new N4_response_message("NO_CODE_EXENSION_NULL", "SEVERE", "CODE_EXTENSON es nulo"));
                return n;

            }
            if (string.IsNullOrEmpty(co.location))
            {
                n.status = 3;
                n.status_id = "SEVERE";
                n.messages.Add(new N4_response_message("NO_LOCATION_FOR_CODE_EXTENSION", "SEVERE", "NO HAY LOCACION"));
                return n;

            }
            if (string.IsNullOrEmpty(co.name))
            {
                n.status = 3;
                n.status_id = "SEVERE";
                n.messages.Add(new N4_response_message("NO_NAME_FOR_CODE_EXTENSION", "SEVERE", "NO HAY NOMBRE"));
                return n;
            }
            var nbb = N4Basic.GetInstance(n4.usuario, n4.password, n4.url, n4.scope);
            var gs = co.ToString();

            var aprr = nbb.BasicInvokeService(gs, p.myClase, p.actualMetodo, usuario, 7000);
            return aprr;
        }

         public static Respuesta.ResultadoOperacion<bool> PonerEventoPasePuertaCFS(Int64 GKEY, string USUARIO)
        {
            string pv;
            var p = InicializaServicio(out pv);
            p.actualMetodo = MethodBase.GetCurrentMethod().Name;
            if (p == null)
            {
                p.LogError<ApplicationException>(new ApplicationException("No fue posible inicializar objeto servicios"), p.actualMetodo, USUARIO);
                //trace log not init
                return Respuesta.ResultadoOperacion<bool>.CrearFalla("La inicialización del objeto N4 falló");
            }
            if (!p.Accesorio.ExistenConfiguraciones)
            {
                return Respuesta.ResultadoOperacion<bool>.CrearFalla("No existen configuraciones");
            }
            var evento = p.Accesorio.ObtenerConfiguracion("PASE_VENCIDO_CFS")?.valor;
            var notas = p.Accesorio.ObtenerConfiguracion("NOTA_EVENTO_CFS")?.valor;
             N4Ws.Entidad.GroovyCodeExtension code = new N4Ws.Entidad.GroovyCodeExtension();
            code.name = "CGSAUnitEventBBKWS";
            code.location = "code-extension";
            code.parameters.Add("UNIT", GKEY.ToString());
            code.parameters.Add("USER", USUARIO);
            code.parameters.Add("NOTES", notas);
            code.parameters.Add("DATE", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            code.parameters.Add("EVENT", evento);
            //Poner el evento
            var n4r = N4Ws.Entidad.Servicios.EjecutarCODEExtensionGenerico(code, USUARIO);
            if (n4r.status != 1)
            {
                var ex = new ApplicationException(n4r.status_id);
                var i =  p.LogError<ApplicationException>(ex, "PonerEventoPasePuertaCFS", USUARIO);
                var emsg = string.Format("Ha ocurrido la novedad número {0} durante el proceso favor comuníquese con facturacion",i.HasValue?i.Value:-1);
                return Respuesta.ResultadoOperacion<bool>.CrearFalla(emsg);
            }
            return Respuesta.ResultadoOperacion<bool>.CrearResultadoExitoso(true,   "Éxito al aplicar el evento");
        }


        public static N4_BasicResponse EjecutarHPU(List<string> unidades, Dictionary<string, bool> holds)
        {
            var n = new N4_BasicResponse();
            //paso 1-> Inicializar instancia de servicio
            string pv;
            var p = InicializaServicio(out pv);
            p.actualMetodo = MethodBase.GetCurrentMethod().Name;
            if (p == null)
            {
                n.status = 3;
                n.status_id = "SEVERE";
                n.messages.Add(new N4_response_message("NO_INITIALIZED_DATA", "SEVERE", pv));
                return n;
            }
            //paso 2 -> inicializar instancia de n4configurariones
            var n4 = ObtenerInicializador(p, out pv);
            if (n4 == null)
            {
                n.status = 3;
                n.status_id = "SEVERE";
                n.messages.Add(new N4_response_message("NO_INITIALIZED_N4_INSTANCE", "SEVERE", pv));
                return n;
            }

            if (unidades == null || unidades.Count <= 0)
            {
                n.status = 3;
                n.status_id = "SEVERE";
                n.messages.Add(new N4_response_message("NO_UNITS_FOR_APPLY_ACTION", "SEVERE", pv));
                return n;
            }

            var rpu = new hpu();
            unidades.ForEach(u => {
                rpu.entities.Add(new unit() { id = u });
            });
            foreach (var h in holds)
            {
                rpu.flags.Add(new flag() { hold_perm_id = h.Key, action= h.Value? _Action.APPLY_HOLD:_Action.RELEASE_HOLD });
            }
            var nbb = N4Basic.GetInstance(n4.usuario, n4.password, n4.url, n4.scope);
            var gs = rpu.ToString();
            n = nbb.BasicInvokeService(gs, p.myClase, p.actualMetodo, "admin", 7000);
            return n;
        }


        public static N4_BasicResponse EjecutarGroovyHoras(PowerLineHour Item, string usuario)
        {
            var n = new N4_BasicResponse();
            //paso 1-> Inicializar instancia de servicio
            string pv;
            var p = InicializaServicio(out pv);
            p.actualMetodo = MethodBase.GetCurrentMethod().Name;
            if (p == null)
            {
                n.status = 3;
                n.status_id = "SEVERE";
                n.messages.Add(new N4_response_message("NO_INITIALIZED_DATA", "SEVERE", pv));
                return n;
            }
            //paso 2 -> inicializar instancia de n4configurariones
            var n4 = ObtenerInicializador(p, out pv);
            if (n4 == null)
            {
                n.status = 3;
                n.status_id = "SEVERE";
                n.messages.Add(new N4_response_message("NO_INITIALIZED_N4_INSTANCE", "SEVERE", pv));
                return n;
            }
            var nbb = N4Basic.GetInstance(n4.usuario, n4.password, n4.url, n4.scope);
            var gs = Item.ToString();
            n = nbb.BasicInvokeService(gs, p.myClase, p.actualMetodo, "admin", 7000);

            var fir = n.response.Descendants().Where(nf => nf.Name.LocalName == "result").FirstOrDefault();
            if (fir != null && !string.IsNullOrEmpty(fir.Value))
            {
                if (!fir.Value.Contains("OK"))
                {
                    n.status = 3;
                    n.status_id = fir.Value;
                }
            }

            return n;
        }


        //nuevo para eventos carbono neutro contenedor
        public static Respuesta.ResultadoOperacion<bool> PonerEventoCarbonoNeutro(List<string> unidades, string usuario)
        {
            string pv;
            var p = InicializaServicio(out pv);
            p.actualMetodo = MethodBase.GetCurrentMethod().Name;
            if (p == null)
            {
                p.LogError<ApplicationException>(new ApplicationException("No fue posible inicializar objeto servicios"), p.actualMetodo, usuario);
                //trace log not init
                return Respuesta.ResultadoOperacion<bool>.CrearFalla("La inicialización del objeto N4 falló");
            }

            if (!p.Accesorio.ExistenConfiguraciones)
            {
                return Respuesta.ResultadoOperacion<bool>.CrearFalla("No existen configuraciones");
            }

            if (unidades == null || unidades.Count <= 0)
            {
                return Respuesta.ResultadoOperacion<bool>.CrearFalla("No existen unidades para agregar evento de carbono neutro");
            }


            var evento = p.Accesorio.ObtenerConfiguracion("CERT_CARBONO")?.valor;
            var notas = p.Accesorio.ObtenerConfiguracion("NOTA_CERTIFICADO")?.valor;
            var propiedad = p.Accesorio.ObtenerConfiguracion("PROPIEDAD")?.valor;
            var tipo = p.Accesorio.ObtenerConfiguracion("TIPO")?.valor;

            var icu = new ICU_API();
            icu.evento = new ICU_EVENT();
            icu.evento.id = evento;
            icu.evento.timeeventapplied = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
            icu.evento.userid = usuario;
            icu.evento.note = notas;



            icu.units = new List<ICU_UNIT>();

            //agreagar conteendores
            unidades.ForEach(u => {
                icu.units.Add(new ICU_UNIT() { id = u, type = tipo });
            });

            icu.properties = new List<ICU_PROPERTY>();
            icu.properties.Add(new ICU_PROPERTY() { tag = propiedad, value = notas });

            var tr = N4ICUService(icu, usuario);
            if (tr.status > 2)
            {
                return Respuesta.ResultadoOperacion<bool>.CrearFalla(tr.status_id);
            }


            return Respuesta.ResultadoOperacion<bool>.CrearResultadoExitoso(true, "Éxito al aplicar el evento");
        }

        //nuevo para eventos carbono neutro cfs
        public static Respuesta.ResultadoOperacion<bool> PonerEventoCarbonoNeutroCFS(Int64 id_unidades, string usuario)
        {
            string pv;
            var p = InicializaServicio(out pv);
            p.actualMetodo = MethodBase.GetCurrentMethod().Name;
            if (p == null)
            {
                p.LogError<ApplicationException>(new ApplicationException("No fue posible inicializar objeto servicios"), p.actualMetodo, usuario);
                //trace log not init
                return Respuesta.ResultadoOperacion<bool>.CrearFalla("La inicialización del objeto N4 falló");
            }

            if (!p.Accesorio.ExistenConfiguraciones)
            {
                return Respuesta.ResultadoOperacion<bool>.CrearFalla("No existen configuraciones");
            }


            var evento = p.Accesorio.ObtenerConfiguracion("CERT_CARBONO_CFS")?.valor;
            var notas = p.Accesorio.ObtenerConfiguracion("NOTA_CERTIFICADO_CFS")?.valor;

            N4Ws.Entidad.GroovyCodeExtension code = new N4Ws.Entidad.GroovyCodeExtension();
            code.name = "CGSAUnitEventBBKWS";
            code.location = "code-extension";
            code.parameters.Add("UNIT", id_unidades.ToString());
            code.parameters.Add("USER", usuario);
            code.parameters.Add("NOTES", notas);
            code.parameters.Add("DATE", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            code.parameters.Add("EVENT", evento);

            //Poner el evento
            var n4r = N4Ws.Entidad.Servicios.EjecutarCODEExtensionGenerico(code, usuario);
            if (n4r.status != 1)
            {
                var ex = new ApplicationException(n4r.status_id);
                var i = p.LogError<ApplicationException>(ex, "PonerEventoCarbonoNeutroCFS", usuario);
                var emsg = string.Format("Ha ocurrido la novedad número {0} durante el proceso favor comuníquese con facturacion", i.HasValue ? i.Value : -1);
                return Respuesta.ResultadoOperacion<bool>.CrearFalla(emsg);
            }

            return Respuesta.ResultadoOperacion<bool>.CrearResultadoExitoso(true, "Éxito al aplicar el evento");
        } 



        //eventos OFFPOWER - ONPOWER
        private static N4_BasicResponse On_Off_Reefer(ReeferMonitor rm, string usuario)
        {
            var n = new N4_BasicResponse();
            if (rm == null)
            {
                n.status = 3;
                n.status_id = "SEVERE";
                n.messages.Add(new N4_response_message("NO_PARAMETERS", "SEVERE", "Entidad es nula On_Off_Reefer"));
                return n;

            }
            //paso 1-> Inicializar instancia de servicio
            string pv;
            var p = InicializaServicio(out pv);
            p.actualMetodo = MethodBase.GetCurrentMethod().Name;
            if (p == null)
            {
                n.status = 3;
                n.status_id = "SEVERE";
                n.messages.Add(new N4_response_message("NO_INITIALIZED_DATA", "SEVERE", pv));
                return n;
            }
            //paso 2 -> inicializar instancia de n4configurariones
            var n4 = ObtenerInicializador(p, out pv);
            if (n4 == null)
            {
                n.status = 3;
                n.status_id = "SEVERE";
                n.messages.Add(new N4_response_message("NO_INITIALIZED_N4_INSTANCE", "SEVERE", pv));
                return n;
            }

            var nbb = N4Basic.GetInstance(n4.usuario, n4.password, n4.url, n4.scope);
            var gs = rm.ToString();
            return nbb.BasicInvokeService(gs, p.myClase, p.actualMetodo, usuario, 7000);
        }
        //eventos OFFPOWER - ONPOWER
        public static Respuesta.ResultadoOperacion<bool> ONPOWER_OFFPOWER(List<string> contenedor, string tipo, string o2, string co2, string humidity, string ventilation, string usuario)
        {
            var s = new Servicios();

            string pv;
            var p = InicializaServicio(out pv);
            p.actualMetodo = MethodBase.GetCurrentMethod().Name;
            if (p == null)
            {
                p.LogError<ApplicationException>(new ApplicationException("No fue posible inicializar objeto servicios"), p.actualMetodo, usuario);
                return Respuesta.ResultadoOperacion<bool>.CrearFalla("La inicialización del objeto N4 falló");
                //trace log not init
            }
            var dd = new Dictionary<string, string>();
            if (contenedor.Count <= 0)
            {
                //save log no count
                p.LogError<ApplicationException>(new ApplicationException("La lista de contenedores es vacia"), p.actualMetodo, usuario);
                return Respuesta.ResultadoOperacion<bool>.CrearFalla("La lista de contenedores es vacia");
            }
            foreach (var g in contenedor)
            {
                var cg = new ReeferMonitor(g.ToString(), tipo, o2, co2, humidity, ventilation);
                var n4r = On_Off_Reefer(cg, usuario);
                if (n4r.status > 2)
                {
                    //log problem
                    p.LogError<ApplicationException>(new ApplicationException(string.Format("{0},{1}", g, n4r.status_id)), p.actualMetodo, usuario);
                    return Respuesta.ResultadoOperacion<bool>.CrearFalla((n4r.status_id==null ? "Error inesperado del sistema al conectarse con n4" : n4r.status_id));
                }
            }

            return Respuesta.ResultadoOperacion<bool>.CrearResultadoExitoso(true, "Éxito al aplicar el evento");
        }



        //eventos MONITOREO REEFER RETURN
        private static N4_BasicResponse Monitoreo_Reefer_Return(ReeferMonitoringReturn rm, string usuario)
        {
            var n = new N4_BasicResponse();
            if (rm == null)
            {
                n.status = 3;
                n.status_id = "SEVERE";
                n.messages.Add(new N4_response_message("NO_PARAMETERS", "SEVERE", "Entidad es nula Monitoreo_Reefer_Return"));
                return n;

            }
            //paso 1-> Inicializar instancia de servicio
            string pv;
            var p = InicializaServicio(out pv);
            p.actualMetodo = MethodBase.GetCurrentMethod().Name;
            if (p == null)
            {
                n.status = 3;
                n.status_id = "SEVERE";
                n.messages.Add(new N4_response_message("NO_INITIALIZED_DATA", "SEVERE", pv));
                return n;
            }
            //paso 2 -> inicializar instancia de n4configurariones
            var n4 = ObtenerInicializador(p, out pv);
            if (n4 == null)
            {
                n.status = 3;
                n.status_id = "SEVERE";
                n.messages.Add(new N4_response_message("NO_INITIALIZED_N4_INSTANCE", "SEVERE", pv));
                return n;
            }

            var nbb = N4Basic.GetInstance(n4.usuario, n4.password, n4.url, n4.scope);
            var gs = rm.ToString();
            return nbb.BasicInvokeService(gs, p.myClase, p.actualMetodo, usuario, 7000);
        }
        public static Respuesta.ResultadoOperacion<bool> REEFER_MONITORING_RETURN(List<string> contenedor, string fecha_hora, string temperatura, 
            string humedad, string temperatura2, string ventilacion, string oxigenacion, string tipo, string usuario)
        {
            var s = new Servicios();

            string pv;
            var p = InicializaServicio(out pv);
            p.actualMetodo = MethodBase.GetCurrentMethod().Name;
            if (p == null)
            {
                p.LogError<ApplicationException>(new ApplicationException("No fue posible inicializar objeto servicios"), p.actualMetodo, usuario);
                return Respuesta.ResultadoOperacion<bool>.CrearFalla("La inicialización del objeto N4 falló");
                //trace log not init
            }
            var dd = new Dictionary<string, string>();
            if (contenedor.Count <= 0)
            {
                //save log no count
                p.LogError<ApplicationException>(new ApplicationException("La lista de contenedores es vacia"), p.actualMetodo, usuario);
                return Respuesta.ResultadoOperacion<bool>.CrearFalla("La lista de contenedores es vacia");
            }
            foreach (var g in contenedor)
            {
                var cg = new ReeferMonitoringReturn(g.ToString(), fecha_hora, temperatura, humedad, temperatura2, ventilacion, oxigenacion, tipo);
                var n4r = Monitoreo_Reefer_Return(cg, usuario);
                if (n4r.status > 2)
                {
                    //log problem
                    p.LogError<ApplicationException>(new ApplicationException(string.Format("{0},{1}", g, n4r.status_id)), p.actualMetodo, usuario);
                    return Respuesta.ResultadoOperacion<bool>.CrearFalla((n4r.status_id == null ? "Error inesperado del sistema al conectarse con n4" : n4r.status_id));
                }
            }

            return Respuesta.ResultadoOperacion<bool>.CrearResultadoExitoso(true, "Éxito al aplicar el evento");
        }

        //eventos MONITOREO REEFER SUPPLY
        private static N4_BasicResponse Monitoreo_Reefer_Suppply(ReeferMonitoringSupply rm, string usuario)
        {
            var n = new N4_BasicResponse();
            if (rm == null)
            {
                n.status = 3;
                n.status_id = "SEVERE";
                n.messages.Add(new N4_response_message("NO_PARAMETERS", "SEVERE", "Entidad es nula ReeferMonitoringSupply"));
                return n;

            }
            //paso 1-> Inicializar instancia de servicio
            string pv;
            var p = InicializaServicio(out pv);
            p.actualMetodo = MethodBase.GetCurrentMethod().Name;
            if (p == null)
            {
                n.status = 3;
                n.status_id = "SEVERE";
                n.messages.Add(new N4_response_message("NO_INITIALIZED_DATA", "SEVERE", pv));
                return n;
            }
            //paso 2 -> inicializar instancia de n4configurariones
            var n4 = ObtenerInicializador(p, out pv);
            if (n4 == null)
            {
                n.status = 3;
                n.status_id = "SEVERE";
                n.messages.Add(new N4_response_message("NO_INITIALIZED_N4_INSTANCE", "SEVERE", pv));
                return n;
            }

            var nbb = N4Basic.GetInstance(n4.usuario, n4.password, n4.url, n4.scope);
            var gs = rm.ToString();
            return nbb.BasicInvokeService(gs, p.myClase, p.actualMetodo, usuario, 7000);
        }
        public static Respuesta.ResultadoOperacion<bool> REEFER_MONITORING_SUPPLY(List<string> contenedor, string fecha_hora, string humedad, 
            string supply_temperatura, string temperatura, string ventilacion, string oxigenacion, string tipo, string usuario)
        {
            var s = new Servicios();

            string pv;
            var p = InicializaServicio(out pv);
            p.actualMetodo = MethodBase.GetCurrentMethod().Name;
            if (p == null)
            {
                p.LogError<ApplicationException>(new ApplicationException("No fue posible inicializar objeto servicios"), p.actualMetodo, usuario);
                return Respuesta.ResultadoOperacion<bool>.CrearFalla("La inicialización del objeto N4 falló");
                //trace log not init
            }
            var dd = new Dictionary<string, string>();
            if (contenedor.Count <= 0)
            {
                //save log no count
                p.LogError<ApplicationException>(new ApplicationException("La lista de contenedores es vacia"), p.actualMetodo, usuario);
                return Respuesta.ResultadoOperacion<bool>.CrearFalla("La lista de contenedores es vacia");
            }
            foreach (var g in contenedor)
            {
                var cg = new ReeferMonitoringSupply(g.ToString(), fecha_hora, humedad, supply_temperatura, temperatura, ventilacion, oxigenacion, tipo);
                var n4r = Monitoreo_Reefer_Suppply(cg, usuario);
                if (n4r.status > 2)
                {
                    //log problem
                    p.LogError<ApplicationException>(new ApplicationException(string.Format("{0},{1}", g, n4r.status_id)), p.actualMetodo, usuario);
                    return Respuesta.ResultadoOperacion<bool>.CrearFalla((n4r.status_id == null ? "Error inesperado del sistema al conectarse con n4" : n4r.status_id));
                }
            }

            return Respuesta.ResultadoOperacion<bool>.CrearResultadoExitoso(true, "Éxito al aplicar el evento");
        }
        //######################################
        //EVENTO LOAD TRUCK - BREAK BULK
        //######################################
        private static N4_BasicResponse LoadTruck_BreakBulk(BreakBulkLoadTruck brbkLT, string usuario)
        {
            var n = new N4_BasicResponse();
            if (brbkLT == null)
            {
                n.status = 3;
                n.status_id = "SEVERE";
                n.messages.Add(new N4_response_message("NO_PARAMETERS", "SEVERE", "Entidad es nula On_Off_Reefer"));
                return n;

            }
            //paso 1-> Inicializar instancia de servicio
            string pv;
            var p = InicializaServicio(out pv);
            p.actualMetodo = MethodBase.GetCurrentMethod().Name;
            if (p == null)
            {
                n.status = 3;
                n.status_id = "SEVERE";
                n.messages.Add(new N4_response_message("NO_INITIALIZED_DATA", "SEVERE", pv));
                return n;
            }
            //paso 2 -> inicializar instancia de n4configurariones
            var n4 = ObtenerInicializador(p, out pv);
            if (n4 == null)
            {
                n.status = 3;
                n.status_id = "SEVERE";
                n.messages.Add(new N4_response_message("NO_INITIALIZED_N4_INSTANCE", "SEVERE", pv));
                return n;
            }

            var nbb = N4Basic.GetInstance(n4.usuario, n4.password, n4.url, n4.scope);
            var gs = brbkLT.ToString();
            return nbb.BasicInvokeService(gs, p.myClase, p.actualMetodo, usuario, 7000);
        }

        public static Respuesta.ResultadoOperacion<bool> DESPACHO_LOADTRUCK(string _bl, string _placa, string _cantidad, string _user)
        {
            var s = new Servicios();

            string pv;
            var p = InicializaServicio(out pv);
            p.actualMetodo = MethodBase.GetCurrentMethod().Name;
            if (p == null)
            {
                p.LogError<ApplicationException>(new ApplicationException("No fue posible inicializar objeto servicios"), p.actualMetodo, _user);
                return Respuesta.ResultadoOperacion<bool>.CrearFalla("La inicialización del objeto N4 falló");
            }
              
            var cg = new BreakBulkLoadTruck( _bl,  _placa,  _cantidad,  _user);
            var n4r = LoadTruck_BreakBulk(cg, _user);
            if (n4r.status > 2)
            {
                //log problem
                p.LogError<ApplicationException>(new ApplicationException(string.Format("{0},{1}", _bl, n4r.status_id)), p.actualMetodo, _user);
                return Respuesta.ResultadoOperacion<bool>.CrearFalla((n4r.status_id == null ? "Error inesperado del sistema al conectarse con n4" : n4r.status_id));
            }
            else
            {
                XDocument xds = XDocument.Parse(n4r.response.ToString());
                string vResult = xds.Root.Descendants("result").FirstOrDefault().Value;

                if (vResult.Contains("ERROR"))
                {
                    p.LogError<ApplicationException>(new ApplicationException(string.Format("{0},{1}", vResult + " " +_bl, n4r.status_id)), p.actualMetodo, _user);
                    return Respuesta.ResultadoOperacion<bool>.CrearFalla((n4r.status_id == null ? "Error inesperado del sistema al conectarse con n4" : vResult));
                    
                }
                else
                {
                    //if (n4r.response.ToString().ToUpper().Contains("OK"))
                    //{
                    //    return Respuesta.ResultadoOperacion<bool>.CrearResultadoExitoso(true, "Éxito al aplicar groovy");
                    //}
                    try
                    {
                        decimal vSaldo = decimal.Parse(vResult);
                        return Respuesta.ResultadoOperacion<bool>.CrearResultadoExitoso(true, "Load Truck con éxito: Cantidad disponible: " + vResult);
                    }
                    catch
                    {
                        p.LogError<ApplicationException>(new ApplicationException(string.Format("{0},{1}", n4r.response.ToString() + " " + _bl, n4r.status_id)), p.actualMetodo, _user);
                        return Respuesta.ResultadoOperacion<bool>.CrearFalla((n4r.status_id == null ? "Error inesperado del sistema al conectarse con n4" : n4r.response.ToString()));
                    }
                }
            }
            
        }

        //######################################
        //EVENTO CGSAUnitLoad - POSITION
        //######################################
        private static N4_BasicResponse PositionUnitLoad(GroovyPOW brbkLT, string usuario)
        {
            var n = new N4_BasicResponse();
            if (brbkLT == null)
            {
                n.status = 3;
                n.status_id = "SEVERE";
                n.messages.Add(new N4_response_message("NO_PARAMETERS", "SEVERE", "Entidad es nula On_Off_Reefer"));
                return n;

            }
            //paso 1-> Inicializar instancia de servicio
            string pv;
            var p = InicializaServicio(out pv);
            p.actualMetodo = MethodBase.GetCurrentMethod().Name;
            if (p == null)
            {
                n.status = 3;
                n.status_id = "SEVERE";
                n.messages.Add(new N4_response_message("NO_INITIALIZED_DATA", "SEVERE", pv));
                return n;
            }
            //paso 2 -> inicializar instancia de n4configurariones
            var n4 = ObtenerInicializador(p, out pv);
            if (n4 == null)
            {
                n.status = 3;
                n.status_id = "SEVERE";
                n.messages.Add(new N4_response_message("NO_INITIALIZED_N4_INSTANCE", "SEVERE", pv));
                return n;
            }

            var nbb = N4Basic.GetInstance(n4.usuario, n4.password, n4.url, n4.scope);
            var gs = brbkLT.ToString();
            return nbb.BasicInvokeService(gs, p.myClase, p.actualMetodo, usuario, 7000);
        }

        public static Respuesta.ResultadoOperacion<bool> POSITION_UNITLOAD(string _pow, string _visit, string _unit, string _position, string _seal, string _user, out string v_request, out string v_response)
        {
            var s = new Servicios();
            string pv;
            v_request = string.Empty;
            v_response = string.Empty;

            var p = InicializaServicio(out pv);
            p.actualMetodo = MethodBase.GetCurrentMethod().Name;
            if (p == null)
            {
                p.LogError<ApplicationException>(new ApplicationException("No fue posible inicializar objeto servicios"), p.actualMetodo, _user);
                return Respuesta.ResultadoOperacion<bool>.CrearFalla("La inicialización del objeto N4 falló");
            }

            var cg = new GroovyPOW(_pow, _visit, _unit, _position, _seal,  _user);
            var n4r = PositionUnitLoad(cg, _user);

            v_request = cg.ToString();
            v_response = n4r.response.ToString();

            if (n4r.status > 2)
            {
                //log problem
                p.LogError<ApplicationException>(new ApplicationException(string.Format("{0}-{1},{2}", _unit,_seal, n4r.status_id)), p.actualMetodo, _user);
                return Respuesta.ResultadoOperacion<bool>.CrearFalla((n4r.status_id == null ? "Error inesperado del sistema al conectarse con n4" : n4r.status_id));
            }
            else
            {
                XDocument xds = XDocument.Parse(n4r.response.ToString());
                string vResult = xds.Root.Descendants("result").FirstOrDefault().Value;

                if (vResult.Contains("ERROR"))
                {
                    p.LogError<ApplicationException>(new ApplicationException(string.Format("{0},{1}", vResult + " " + _unit, n4r.status_id)), p.actualMetodo, _user);
                    return Respuesta.ResultadoOperacion<bool>.CrearFalla((n4r.status_id == null ? "Error inesperado del sistema al conectarse con n4" : vResult));

                }
                else
                {
                    //if (n4r.response.ToString().ToUpper().Contains("OK"))
                    //{
                    //    return Respuesta.ResultadoOperacion<bool>.CrearResultadoExitoso(true, "Éxito al aplicar groovy");
                    //}
                    try
                    {
                        decimal vSaldo = decimal.Parse(vResult);
                        return Respuesta.ResultadoOperacion<bool>.CrearResultadoExitoso(true, "Transacción con Éxito POSITION_UNITLOAD: " + _unit + " TState -> Loaded");
                    }
                    catch
                    {
                        p.LogError<ApplicationException>(new ApplicationException(string.Format("{0},{1}", n4r.response.ToString() + " " + _unit, n4r.status_id)), p.actualMetodo, _user);
                        return Respuesta.ResultadoOperacion<bool>.CrearFalla((n4r.status_id == null ? "Error inesperado del sistema al conectarse con n4" : n4r.response.ToString()));
                    }
                }
            }

        }

        //######################################
        //ICU HOLD
        //######################################
        public static N4_BasicResponse N4ICU_HOLD(string unitID,bool estado, string usuario, string _hold)
        {
            var n = new N4_BasicResponse();
            string pv;
            var p = InicializaServicio(out pv);
            p.actualMetodo = MethodBase.GetCurrentMethod().Name;
            if (p == null)
            {
                n.status = 3;
                n.status_id = "SEVERE";
                n.messages.Add(new N4_response_message("NO_INITIALIZED_DATA", "SEVERE", pv));
                return n;
            }
            //paso 2 -> inicializar instancia de n4configurariones
            var n4 = ObtenerInicializador(p, out pv);
            if (n4 == null)
            {
                n.status = 3;
                n.status_id = "SEVERE";
                n.messages.Add(new N4_response_message("NO_INITIALIZED_N4_INSTANCE", "SEVERE", pv));
                return n;
            }
            if (string.IsNullOrWhiteSpace(unitID))
            {
                n.status = 3;
                n.status_id = "SEVERE";
                n.messages.Add(new N4_response_message("NO_ICU_INSTANCE", "SEVERE", "NO SE ENCONTRO OBJETO ICU DE API"));
                return n;
            }
            var nbb = N4Basic.GetInstance(n4.usuario, n4.password, n4.url, n4.scope);

            var texto = new StringBuilder();
            texto.Append("<hpu><entities><units>");
            texto.AppendFormat("<unit-identity id=\"{0}\" ></unit-identity>", unitID);
            texto.AppendFormat("</units></entities><flags><flag hold-perm-id=\"{1}\" action=\"{0}\"/></flags></hpu>", (estado) ? "GRANT_PERMISSION" : "CANCEL_PERMISSION", _hold/*"CUSTOM_SEALS_MTY"*/);

            // return nbb.BasicInvokeService<ICU_API>(icu, usuario, p.myClase, p.actualMetodo, 7500);
            var n4Result =  nbb.BasicInvokeService(texto.ToString(), p.myClase, p.actualMetodo, usuario, 7000);
            
            return n4Result;

        }

        //######################################
        //EVENTO CGSAUnitDischarge - DISCHARGE
        //######################################
        private static N4_BasicResponse XMLConfirmacionDescarga(GroovyDischarge oGroovy, string usuario)
        {
            var n = new N4_BasicResponse();
            if (oGroovy == null)
            {
                n.status = 3;
                n.status_id = "SEVERE";
                n.messages.Add(new N4_response_message("NO_PARAMETERS", "SEVERE", "Entidad es nula On_Off_Reefer"));
                return n;

            }
            //paso 1-> Inicializar instancia de servicio
            string pv;
            var p = InicializaServicio(out pv);
            p.actualMetodo = MethodBase.GetCurrentMethod().Name;
            if (p == null)
            {
                n.status = 3;
                n.status_id = "SEVERE";
                n.messages.Add(new N4_response_message("NO_INITIALIZED_DATA", "SEVERE", pv));
                return n;
            }
            //paso 2 -> inicializar instancia de n4configurariones
            var n4 = ObtenerInicializador(p, out pv);
            if (n4 == null)
            {
                n.status = 3;
                n.status_id = "SEVERE";
                n.messages.Add(new N4_response_message("NO_INITIALIZED_N4_INSTANCE", "SEVERE", pv));
                return n;
            }

            var nbb = N4Basic.GetInstance(n4.usuario, n4.password, n4.url, n4.scope);
            var gs = oGroovy.ToString();
            return nbb.BasicInvokeService(gs, p.myClase, p.actualMetodo, usuario, 7000);
        }

        public static Respuesta.ResultadoOperacion<bool> DISCHARGE( string _visit, string _unit, string _position, string _user, out string v_request, out string v_response)
        {
            var s = new Servicios();
            string pv;
            v_request = string.Empty;
            v_response = string.Empty;

            var p = InicializaServicio(out pv);
            p.actualMetodo = MethodBase.GetCurrentMethod().Name;
            if (p == null)
            {
                p.LogError<ApplicationException>(new ApplicationException("No fue posible inicializar objeto servicios"), p.actualMetodo, _user);
                return Respuesta.ResultadoOperacion<bool>.CrearFalla("La inicialización del objeto N4 falló");
            }

            var cg = new GroovyDischarge( _visit, _unit, _position,  _user);
            var n4r = XMLConfirmacionDescarga(cg, _user);

            v_request = cg.ToString();
            v_response = n4r.response.ToString();

            if (n4r.status > 2)
            {
                //log problem
                p.LogError<ApplicationException>(new ApplicationException(string.Format("{0}-{1},{2}", _unit, _position, n4r.status_id)), p.actualMetodo, _user);
                return Respuesta.ResultadoOperacion<bool>.CrearFalla((n4r.status_id == null ? "Error inesperado del sistema al conectarse con n4" : n4r.status_id));
            }
            else
            {
                XDocument xds = XDocument.Parse(n4r.response.ToString());
                string vResult = xds.Root.Descendants("result").FirstOrDefault().Value;

                if (vResult.Contains("ERROR"))
                {
                    p.LogError<ApplicationException>(new ApplicationException(string.Format("{0},{1}", vResult + " " + _unit, n4r.status_id)), p.actualMetodo, _user);
                    return Respuesta.ResultadoOperacion<bool>.CrearFalla((n4r.status_id == null ? "Error inesperado del sistema al conectarse con n4" : vResult));

                }
                else
                {
                    //if (n4r.response.ToString().ToUpper().Contains("OK"))
                    //{
                    //    return Respuesta.ResultadoOperacion<bool>.CrearResultadoExitoso(true, "Éxito al aplicar groovy");
                    //}
                    try
                    {
                        decimal vSaldo = decimal.Parse(vResult);
                        return Respuesta.ResultadoOperacion<bool>.CrearResultadoExitoso(true, "Transacción con éxito DISCHARGE: " + _unit + " TState -> Yard");
                    }
                    catch
                    {
                        p.LogError<ApplicationException>(new ApplicationException(string.Format("{0},{1}", n4r.response.ToString() + " " + _unit, n4r.status_id)), p.actualMetodo, _user);
                        return Respuesta.ResultadoOperacion<bool>.CrearFalla((n4r.status_id == null ? "Error inesperado del sistema al conectarse con n4" : n4r.response.ToString()));
                    }
                }
            }

        }


        //nuevo para evento de transporte P2D
        public static Respuesta.ResultadoOperacion<bool> PonerEvento_Paletizado(Int64 id_unidades, string usuario, int qty)
        {
            string pv;
            var p = InicializaServicio(out pv);
            p.actualMetodo = MethodBase.GetCurrentMethod().Name;
            if (p == null)
            {
                p.LogError<ApplicationException>(new ApplicationException("No fue posible inicializar objeto servicios"), p.actualMetodo, usuario);
                //trace log not init
                return Respuesta.ResultadoOperacion<bool>.CrearFalla("La inicialización del objeto N4 falló");
            }

            if (!p.Accesorio.ExistenConfiguraciones)
            {
                return Respuesta.ResultadoOperacion<bool>.CrearFalla("No existen configuraciones");
            }


            var evento = p.Accesorio.ObtenerConfiguracion("SERVICIO_PALLET")?.valor;
            var notas = p.Accesorio.ObtenerConfiguracion("PALLET_NOTA")?.valor;

            N4Ws.Entidad.GroovyCodeExtension code = new N4Ws.Entidad.GroovyCodeExtension();
            code.name = "CGSAUnitEventBBKWSQT";
            code.location = "code-extension";
            code.parameters.Add("UNIT", id_unidades.ToString());
            code.parameters.Add("USER", usuario);
            code.parameters.Add("NOTES", notas);
            code.parameters.Add("QTY", qty.ToString());
            code.parameters.Add("DATE", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            code.parameters.Add("EVENT", evento);

            //Poner el evento
            var n4r = N4Ws.Entidad.Servicios.EjecutarCODEExtensionGenerico(code, usuario);
            if (n4r.status != 1)
            {
                var ex = new ApplicationException(n4r.status_id);
                var i = p.LogError<ApplicationException>(ex, "PonerEvento_Paletizado", usuario);
                var emsg = string.Format("Ha ocurrido la novedad número {0} durante el proceso favor comuníquese con sistemas", i.HasValue ? i.Value : -1);
                return Respuesta.ResultadoOperacion<bool>.CrearFalla(emsg);
            }

            return Respuesta.ResultadoOperacion<bool>.CrearResultadoExitoso(true, "Éxito al aplicar el evento");
        }


    }

}
