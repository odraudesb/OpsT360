using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using Configuraciones;


namespace N4Ws.Entidad
{
    public enum Action { INQUIRE, DRAFT }
    public enum Requester { QUERY_CHARGES }
 

    [XmlRoot("billing")]
    public class billing
    {
        public billing()
        {
            this.Request = new InvoiceRequest();
         
        }
        [XmlElement("generate-invoice-request")]
        public InvoiceRequest Request { get; set; }
        [XmlElement("finalize-invoice-request")]
        public FinalizeInvoiceRequest FinalizeRequest { get; set; }

        [XmlElement("merge-invoices-to-new-invoice-request")]
        public MergeInvoiceRequest MergeInvoiceRequest { get; set; }
    }

    public class InvoiceRequest
    {
        [XmlElement("action")]
        public Action action { get; set; }
        [XmlElement("requester")]
        public Requester requester { get; set; }
        [XmlElement("invoiceTypeId")]
        public string InvoiceTypeId { get; set; }
        [XmlElement("payeeCustomerId")]
        public string payeeCustomerId { get; set; }
        [XmlElement("payeeCustomerBizRole")]
        public string payeeCustomerBizRole { get; set; }
        [XmlElement("currencyId")]
        public string currencyId { get; set; }
        [XmlElement("contractEffectiveDate")]
        public string contractEffectiveDate { get; set; }
        [XmlElement("isInvoiceFinal")]
        public string isInvoiceFinal { get; set; }

        [XmlElement("bexuBlNbr")]
        public string bexuBlNbr { get; set; }

        [XmlArray("invoiceParameters")]
        [XmlArrayItem("invoiceParameter", typeof(invoiceParameter))]
        public List<invoiceParameter> invoiceParameters { get; set; }


        [XmlArray("billToParty")]
        [XmlArrayItem("address", typeof(address))]
        public List<address> billToParty { get; set; }

        public InvoiceRequest()
        {
            this.invoiceParameters = new List<invoiceParameter>();
            this.contractEffectiveDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
            this.billToParty = new List<address>();
            this.currencyId = "USD";
        }
     

    }

    public class invoiceParameter
    {
        [XmlElement("EquipmentId")]
        public string EquipmentId { get; set; }
        [XmlElement("PaidThruDay")]
        public string PaidThruDay { get; set; }
        //-->FOR EXPO DATA: CONTENEDORES, BOOKING, REFERENCIA
        [XmlElement("bexuEqId")]
        public string bexuEqId { get; set; }
        [XmlElement("bexuBookingNbr")]
        public string bexuBookingNbr { get; set; }
        [XmlElement("bexuObId")]
        public string bexuObId { get; set; }

        //-->FOR CFS Impo
        [XmlElement("bexuBlNbr")]
        public string bexuBlNbr { get; set; }
        [XmlElement("bexuPaidThruDay")]
        public string bexuPaidThruDay { get; set; }
        [XmlElement("bexuCategory")]
        public string bexuCategory { get; set; }





    }

    public class address
    {
        [XmlElement("contactName")]
        public string contactName { get; set; }
        [XmlElement("addressLine1")]
        public string addressLine1 { get; set; }
        [XmlElement("addressLine2")]
        public string addressLine2 { get; set; }
        [XmlElement("addressLine3")]
        public string addressLine3 { get; set; }
        [XmlElement("city")]
        public string city { get; set; }
        [XmlElement("mailCode")]
        public string mailCode { get; set; }
        [XmlElement("state")]
        public string state { get; set; }
        [XmlElement("country")]
        public string country { get; set; }
        [XmlElement("telephone")]
        public string telephone { get; set; }
        [XmlElement("fax")]
        public string fax { get; set; }
        [XmlElement("emailAddress")]
        public string emailAddress { get; set; }
    }


    public class FinalizeInvoiceRequest
    {
        [XmlElement("drftInvoiceNbr")]
        public string drftInvoiceNbr { get; set; }
        [XmlElement("finalizeDate")]
        public string finalizeDate { get; set; }

        public override string ToString()
        {
            StringBuilder V = new StringBuilder();
            V.Append("<billing>");
            V.Append("<finalize-invoice-request>");
            if (!string.IsNullOrEmpty(drftInvoiceNbr)) {
                V.AppendFormat("<drftInvoiceNbr>{0}<drftInvoiceNbr>",drftInvoiceNbr);
            }
            if (!string.IsNullOrEmpty(finalizeDate))
            {
                V.AppendFormat("<finalizeDate>{0}<finalizeDate>", finalizeDate);
            }
            V.Append("</finalize-invoice-request>");
            V.Append("</billing>");


            return V.ToString();
        }
    }


    public class FinalizeInvoiceResponse
    {
        [XmlElement("invoice-final-nbr")]
        public string invoiceFinalNumber { get; set; }
    }


    public class MergeInvoiceRequest
    {
        [XmlElement("invoiceTypeId")]
        public string invoiceTypeId { get; set; }

        [XmlElement("finalizeDate")]
        public string finalizeDate { get; set; }
        
        [XmlArray("drftInvoiceNbrs")]
       [XmlArrayItem("drftInvoiceNbr", typeof(String))]
        public List<String> drftInvoiceNbrs { get; set; }

        public MergeInvoiceRequest()
        {
            this.drftInvoiceNbrs = new List<String>();
        }
    }



    /*<argo-response status="0" status-id="OK">
  <argo:finalize-invoice-response xmlns:argo="http://www.navis.com/argo">
    <invoice-final-nbr>1018000396451</invoice-final-nbr>
  </argo:finalize-invoice-response>
</argo-response>

     * 
     * 
     * 
     <billing>
  <finalize-invoice-request>
    <drftInvoiceNbr>1681841</drftInvoiceNbr>
    <finalizeDate>2019-11-29 03:23:00</finalizeDate>
  </finalize-invoice-request>
</billing>

     */
}
