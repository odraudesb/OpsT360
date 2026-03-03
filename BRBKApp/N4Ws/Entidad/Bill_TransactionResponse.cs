using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace N4Ws.Entidad
{
    [Serializable]
    [XmlRoot(ElementName = "N4_Bill_TransactionReponse")]
    public class N4_Bill_TransactionReponse<T>
    {
        [XmlAttribute(AttributeName = "status")]
        public int status { get; set; }

        [XmlAttribute(AttributeName = "status_id")]
        public string status_id { get; set; }

        [XmlArray("messages")]
        [XmlArrayItem("message", typeof(N4_response_message))]
        public List<N4_response_message> messages { get; set; }

        [XmlIgnore]
        public T response { get; set; }

        [XmlIgnore]
        public Int64? trace { get; set; }

        public N4_Bill_TransactionReponse()
        {
            this.messages = new List<N4_response_message>();
        }
        public N4_Bill_TransactionReponse(int _stat, string _stat_id)
        {
            this.status = _stat;
            this.status_id = _stat_id;
            this.messages = new List<N4_response_message>();
        }

        public static string ObtenDataElemento(XElement xe, string nombre)
        {
           return  xe.Attributes().Where(g => g.Name.LocalName.ToLower() == nombre?.ToLower())?.FirstOrDefault()?.Value;
        }
        internal static List<billingTransaction> SerialResponse(XDocument doc )
        {
            var ntlist = new List<billingTransaction>();
            if (doc == null)
            {
                return null;
            }
            //obtener Root->billingTransactions
            var billingTransactions = (from p in doc.Root.Descendants()
                                       where p.Name.LocalName == "billingTransactions"
                                       select p).FirstOrDefault();

            //billingTransaction->Array of billingTransaction
            var billingTransaction = from p in billingTransactions.Descendants()
                        where p.Name.LocalName == "billingTransaction"
                        select p;

            if (billingTransaction == null)
            {
                return null;
            }

            //Por cada Transaccion, llenar objeto
            foreach (var bi in billingTransaction)
            {
                var nn = new billingTransaction();
                //cabeceras de respuesta.
                nn.timestamp = ObtenDataElemento(bi, "timestamp");
                nn.msgReferenceNbr = ObtenDataElemento(bi, "msgReferenceNbr");
                nn.transactionSetNbr = ObtenDataElemento(bi, "transactionSetNbr");
                nn.totalChargedAmount = ObtenDataElemento(bi, "totalChargedAmount");

                //nodo invoice


                //nodo invoice
                var _invoice = (from p in bi.Descendants()
                                where p.Name.LocalName == "invoice"
                                select p).FirstOrDefault();
                //set propiedades de invoice
                             

                nn.billInvoice.gkey = ObtenDataElemento(_invoice, "gkey");
                nn.billInvoice.changed = ObtenDataElemento(_invoice, "changed");
                nn.billInvoice.created = ObtenDataElemento(_invoice, "created");
                nn.billInvoice.draftNumber = ObtenDataElemento(_invoice, "draftNumber");
                nn.billInvoice.dueDate = ObtenDataElemento(_invoice, "dueDate");
                nn.billInvoice.effectiveDate = ObtenDataElemento(_invoice, "effectiveDate");
                nn.billInvoice.isMerged = ObtenDataElemento(_invoice, "isMerged");
                nn.billInvoice.facilityId = ObtenDataElemento(_invoice, "facilityId");
                nn.billInvoice.facilityName = ObtenDataElemento(_invoice, "facilityName");
                nn.billInvoice.complexName = ObtenDataElemento(_invoice, "complexName");
                nn.billInvoice.complexId = ObtenDataElemento(_invoice, "complexId");

                nn.billInvoice.finalizedDate = ObtenDataElemento(_invoice, "finalizedDate");
                nn.billInvoice.finalNumber = ObtenDataElemento(_invoice, "finalNumber");
                nn.billInvoice.revenueMonth = ObtenDataElemento(_invoice, "revenueMonth");
                nn.billInvoice.status = ObtenDataElemento(_invoice, "status");
                nn.billInvoice.totalCharges = ObtenDataElemento(_invoice, "totalCharges");
                nn.billInvoice.totalCredits = ObtenDataElemento(_invoice, "totalCredits");
               // nn.billInvoice.totalChargedAmount = "";

                nn.billInvoice.totalCreditTaxes = ObtenDataElemento(_invoice, "totalCreditTaxes");

                nn.billInvoice.totalDiscounts = ObtenDataElemento(_invoice, "totalDiscounts");
                nn.billInvoice.totalOwed = ObtenDataElemento(_invoice, "totalOwed");
                nn.billInvoice.totalPayments = ObtenDataElemento(_invoice, "totalPayments");
                nn.billInvoice.totalTaxes = ObtenDataElemento(_invoice, "totalTaxes");
                nn.billInvoice.totalTotal = ObtenDataElemento(_invoice, "totalTotal");
                nn.billInvoice.type = ObtenDataElemento(_invoice, "type");
                nn.billInvoice.currency = ObtenDataElemento(_invoice, "currency");
                nn.billInvoice.notes = ObtenDataElemento(_invoice, "notes");
                nn.billInvoice.gkey = ObtenDataElemento(_invoice, "gkey");
                nn.billInvoice.customerReference = ObtenDataElemento(_invoice, "customerReference");

                var _invoiceParams = from p in bi.Descendants()
                                     where p.Name.LocalName == "invoiceParm"
                                     select p;
                //llenar arreglo de invoice_params
                foreach (var ip in _invoiceParams)
                {
                    var ap = new invoiceParm();
                    ap.Metafield = ObtenDataElemento(ip, "Metafield");
                    ap.UiValue = ObtenDataElemento(ip, "UiValue");
                    ap.Value = ObtenDataElemento(ip, "Value"); ;
                    nn.billInvoice.invoiceParameters.Add(ap);
                }

                var _invoiceCharges = from p in bi.Descendants()
                                      where p.Name.LocalName == "invoiceCharge"
                                      select p;

                foreach (var ic in _invoiceCharges)
                {
                    var ap = new invoiceCharge();

                    ap.totalCharged = ObtenDataElemento(ic, "totalCharged");
                    ap.billingDate = ObtenDataElemento(ic, "billingDate");
                    ap.description = ObtenDataElemento(ic, "description");
                    ap.chargeEntityId = ObtenDataElemento(ic, "chargeEntityId");
                    ap.chargeEventTypeId = ObtenDataElemento(ic, "chargeEventTypeId");

                    ap.customerTariffId = ObtenDataElemento(ic, "customerTariffId");
                    ap.notes = ObtenDataElemento(ic, "notes");
                    ap.flatRateAmount = ObtenDataElemento(ic, "flatRateAmount");
                    ap.eventPerformedFrom = ObtenDataElemento(ic, "eventPerformedFrom");
                    ap.chargeGlCode = ObtenDataElemento(ic, "chargeGlCode");


                    ap.eventPerformedTo = ObtenDataElemento(ic, "eventPerformedTo");
                    ap.gkey = ObtenDataElemento(ic, "gkey");
                    ap.extractGkey = ObtenDataElemento(ic, "extractGkey");
                    ap.extractClass = ObtenDataElemento(ic, "extractClass");
                    ap.qtyDivisor = ObtenDataElemento(ic, "qtyDivisor");
                    ap.quantity = ObtenDataElemento(ic, "quantity");


                    ap.quantityBilled = ObtenDataElemento(ic, "quantityBilled");
                    ap.quantityUnit = ObtenDataElemento(ic, "quantityUnit");
                    ap.rateBilled = ObtenDataElemento(ic, "rateBilled");
                    ap.isFlatRate = ObtenDataElemento(ic, "isFlatRate");
                    ap.rateMaxAmount = ObtenDataElemento(ic, "rateMaxAmount");
                    ap.rateMinAmount = ObtenDataElemento(ic, "rateMinAmount");
                    ap.totalTaxes = ObtenDataElemento(ic, "totalTaxes");

                    var ct =  ic.Descendants().Where(it => it.Name.LocalName == "chargeTariff").FirstOrDefault();
                   

                    ap.chargeTariff.id = ObtenDataElemento(ct, "id");
                    ap.chargeTariff.glcode = ObtenDataElemento(ct, "glcode");
                    ap.chargeTariff.longDescription = ObtenDataElemento(ct, "longDescription");

                    var tx = ct.Descendants().Where(it => it.Name.LocalName == "chargeTax").FirstOrDefault();

                    ap.chargeTax.amount = ObtenDataElemento(ct, "amount");
                    ap.chargeTax.date = ObtenDataElemento(ct, "date");
                    ap.chargeTax.rate = ObtenDataElemento(ct, "rate");
                    ap.chargeTax.taxItemId = ObtenDataElemento(ct, "taxItemId");
                     var tr= ct.Descendants().Where(it => it.Name.LocalName == "tariffRate").FirstOrDefault();
                    ap.chargeTariff.tariffRate.contractName = ObtenDataElemento(tr, "contractName");
                    ap.chargeTariff.tariffRate.currency = ObtenDataElemento(tr, "currency");
                    ap.chargeTariff.tariffRate.effectiveDate = ObtenDataElemento(tr, "effectiveDate");
                 //   ap.chargeTariff.tariffRate = trr;
                 //   ap.chargeTariff = ctt;
                    nn.billInvoice.invoiceCharges.Add(ap);
                }

                ntlist.Add(nn);
            }
            return ntlist;
        }


        internal static billingTransaction BillingTransactionData(XDocument doc)
        {
            var nn = new billingTransaction();
            if (doc == null)
            {
                return null;
            }
            //obtener Root->billingTransactions
            var billingTransactions = (from p in doc.Root.Descendants()
                                       where p.Name.LocalName == "billingTransactions"
                                       select p).FirstOrDefault();

            //billingTransaction->Array of billingTransaction
            var bi = (from p in billingTransactions.Descendants()
                                     where p.Name.LocalName == "billingTransaction"
                                     select p).FirstOrDefault();

            if (bi == null)
            {
                return null;
            }
                //cabeceras de respuesta.
                nn.timestamp = ObtenDataElemento(bi, "timestamp");
                nn.msgReferenceNbr = ObtenDataElemento(bi, "msgReferenceNbr");
                nn.transactionSetNbr = ObtenDataElemento(bi, "transactionSetNbr");
                nn.totalChargedAmount = ObtenDataElemento(bi, "totalChargedAmount");

                //nodo invoice


                //nodo invoice
                var _invoice = (from p in bi.Descendants()
                                where p.Name.LocalName == "invoice"
                                select p).FirstOrDefault();
                //set propiedades de invoice


                nn.billInvoice.gkey = ObtenDataElemento(_invoice, "gkey");
                nn.billInvoice.changed = ObtenDataElemento(_invoice, "changed");
                nn.billInvoice.created = ObtenDataElemento(_invoice, "created");
                nn.billInvoice.draftNumber = ObtenDataElemento(_invoice, "draftNumber");
                nn.billInvoice.dueDate = ObtenDataElemento(_invoice, "dueDate");
                nn.billInvoice.effectiveDate = ObtenDataElemento(_invoice, "effectiveDate");
                nn.billInvoice.isMerged = ObtenDataElemento(_invoice, "isMerged");
                nn.billInvoice.facilityId = ObtenDataElemento(_invoice, "facilityId");
                nn.billInvoice.facilityName = ObtenDataElemento(_invoice, "facilityName");
                nn.billInvoice.complexName = ObtenDataElemento(_invoice, "complexName");
                nn.billInvoice.complexId = ObtenDataElemento(_invoice, "complexId");

                nn.billInvoice.finalizedDate = ObtenDataElemento(_invoice, "finalizedDate");
                nn.billInvoice.finalNumber = ObtenDataElemento(_invoice, "finalNumber");
                nn.billInvoice.revenueMonth = ObtenDataElemento(_invoice, "revenueMonth");
                nn.billInvoice.status = ObtenDataElemento(_invoice, "status");
                nn.billInvoice.totalCharges = ObtenDataElemento(_invoice, "totalCharges");
                nn.billInvoice.totalCredits = ObtenDataElemento(_invoice, "totalCredits");
                // nn.billInvoice.totalChargedAmount = "";

                nn.billInvoice.totalCreditTaxes = ObtenDataElemento(_invoice, "totalCreditTaxes");

                nn.billInvoice.totalDiscounts = ObtenDataElemento(_invoice, "totalDiscounts");
                nn.billInvoice.totalOwed = ObtenDataElemento(_invoice, "totalOwed");
                nn.billInvoice.totalPayments = ObtenDataElemento(_invoice, "totalPayments");
                nn.billInvoice.totalTaxes = ObtenDataElemento(_invoice, "totalTaxes");
                nn.billInvoice.totalTotal = ObtenDataElemento(_invoice, "totalTotal");
                nn.billInvoice.type = ObtenDataElemento(_invoice, "type");
                nn.billInvoice.currency = ObtenDataElemento(_invoice, "currency");
                nn.billInvoice.notes = ObtenDataElemento(_invoice, "notes");
                nn.billInvoice.gkey = ObtenDataElemento(_invoice, "gkey");
                nn.billInvoice.customerReference = ObtenDataElemento(_invoice, "customerReference");

                var _invoiceParams = from p in bi.Descendants()
                                     where p.Name.LocalName == "invoiceParm"
                                     select p;
                //llenar arreglo de invoice_params
                foreach (var ip in _invoiceParams)
                {
                    var ap = new invoiceParm();
                    ap.Metafield = ObtenDataElemento(ip, "Metafield");
                    ap.UiValue = ObtenDataElemento(ip, "UiValue");
                    ap.Value = ObtenDataElemento(ip, "Value"); ;
                    nn.billInvoice.invoiceParameters.Add(ap);
                }

                var _invoiceCharges = from p in bi.Descendants()
                                      where p.Name.LocalName == "invoiceCharge"
                                      select p;

                foreach (var ic in _invoiceCharges)
                {
                    var ap = new invoiceCharge();

                    ap.totalCharged = ObtenDataElemento(ic, "totalCharged");
                    ap.billingDate = ObtenDataElemento(ic, "billingDate");
                    ap.description = ObtenDataElemento(ic, "description");
                    ap.chargeEntityId = ObtenDataElemento(ic, "chargeEntityId");
                    ap.chargeEventTypeId = ObtenDataElemento(ic, "chargeEventTypeId");

                    ap.customerTariffId = ObtenDataElemento(ic, "customerTariffId");
                    ap.notes = ObtenDataElemento(ic, "notes");
                    ap.flatRateAmount = ObtenDataElemento(ic, "flatRateAmount");
                    ap.eventPerformedFrom = ObtenDataElemento(ic, "eventPerformedFrom");
                    ap.chargeGlCode = ObtenDataElemento(ic, "chargeGlCode");


                    ap.eventPerformedTo = ObtenDataElemento(ic, "eventPerformedTo");
                    ap.gkey = ObtenDataElemento(ic, "gkey");
                    ap.extractGkey = ObtenDataElemento(ic, "extractGkey");
                    ap.extractClass = ObtenDataElemento(ic, "extractClass");
                    ap.qtyDivisor = ObtenDataElemento(ic, "qtyDivisor");
                    ap.quantity = ObtenDataElemento(ic, "quantity");


                    ap.quantityBilled = ObtenDataElemento(ic, "quantityBilled");
                    ap.quantityUnit = ObtenDataElemento(ic, "quantityUnit");
                    ap.rateBilled = ObtenDataElemento(ic, "rateBilled");
                    ap.isFlatRate = ObtenDataElemento(ic, "isFlatRate");
                    ap.rateMaxAmount = ObtenDataElemento(ic, "rateMaxAmount");
                    ap.rateMinAmount = ObtenDataElemento(ic, "rateMinAmount");
                    ap.totalTaxes = ObtenDataElemento(ic, "totalTaxes");

                    var ct = ic.Descendants().Where(it => it.Name.LocalName == "chargeTariff").FirstOrDefault();


                    ap.chargeTariff.id = ObtenDataElemento(ct, "id");
                    ap.chargeTariff.glcode = ObtenDataElemento(ct, "glcode");
                    ap.chargeTariff.longDescription = ObtenDataElemento(ct, "longDescription");

                    var tx = ct.Descendants().Where(it => it.Name.LocalName == "chargeTax").FirstOrDefault();

                    ap.chargeTax.amount = ObtenDataElemento(ct, "amount");
                    ap.chargeTax.date = ObtenDataElemento(ct, "date");
                    ap.chargeTax.rate = ObtenDataElemento(ct, "rate");
                    ap.chargeTax.taxItemId = ObtenDataElemento(ct, "taxItemId");
                    var tr = ct.Descendants().Where(it => it.Name.LocalName == "tariffRate").FirstOrDefault();
                    ap.chargeTariff.tariffRate.contractName = ObtenDataElemento(tr, "contractName");
                    ap.chargeTariff.tariffRate.currency = ObtenDataElemento(tr, "currency");
                    ap.chargeTariff.tariffRate.effectiveDate = ObtenDataElemento(tr, "effectiveDate");
                    nn.billInvoice.invoiceCharges.Add(ap);
                }
            return nn;
        }



        internal static FinalizeInvoiceResponse FinalizeSerialResponse(XDocument doc)
        {
            //obtener Root->billingTransactions
            var oresponse = (from p in doc.Root.Descendants()
                                       where p.Name.LocalName == "finalize-invoice-response"
                                       select p).FirstOrDefault();
            if (oresponse == null)
            {
                return null;
            }
            var bilfil = (from p in oresponse.Descendants()
                          where p.Name.LocalName == "invoice-final-nbr"
                          select p).FirstOrDefault();

            
            var o = new FinalizeInvoiceResponse();
            o.invoiceFinalNumber = bilfil?.Value;
            return o;
        }



        internal static String FinalizarNumeroFinal(XDocument doc)
        {
            //obtener Root->billingTransactions
            var oresponse = (from p in doc.Root.Descendants()
                             where p.Name.LocalName == "finalize-invoice-response"
                             select p).FirstOrDefault();
            if (oresponse == null)
            {
                return null;
            }
            var bilfil = (from p in oresponse.Descendants()
                          where p.Name.LocalName == "invoice-final-nbr"
                          select p).FirstOrDefault();
            return bilfil?.Value;
        }
    }


    


}
