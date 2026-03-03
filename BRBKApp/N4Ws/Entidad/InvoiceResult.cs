using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace N4Ws.Entidad
{
    public class billingTransaction
    {
        public string msgReferenceNbr { get; set; }
        public string transactionSetNbr { get; set; }
        public string timestamp { get; set; }
        public string totalChargedAmount { get; set; }

        public billInvoice billInvoice { get; set; }

        public billingTransaction()
        {
            billInvoice = new billInvoice();
        }
    }

    public class billInvoice
    {
        public string changed { get; set; }
        public string created { get; set; }
        public string draftNumber { get; set; }
        public string dueDate { get; set; }
        public string effectiveDate { get; set; }
        public string isMerged { get; set; }
        public string facilityId { get; set; }
        public string facilityName { get; set; }
        public string complexName { get; set; }
        public string complexId { get; set; }
        public string finalizedDate { get; set; }
        public string finalNumber { get; set; }
        public string revenueMonth { get; set; }
        public string status { get; set; }
        public string totalCharges { get; set; }
        public string totalCredits { get; set; }
        public string totalCreditTaxes { get; set; }
        public string totalDiscounts { get; set; }
        public string totalOwed { get; set; }
        public string totalPayments { get; set; }
        public string totalTaxes { get; set; }
        public string totalTotal { get; set; }
        public string type { get; set; }
        public string currency { get; set; }
        public string notes { get; set; }
        public string gkey { get; set; }

       
        public string customerReference { get; set; }
        public List<invoiceParm> invoiceParameters { get; set; }
        public List<invoiceCharge> invoiceCharges { get; set; }
        public billInvoice()
        {
            this.invoiceParameters = new List<invoiceParm>();
            this.invoiceCharges = new List<invoiceCharge>();
        }
    }

    public class invoiceParm
    {
        public string Metafield { get; set; }
        public string UiValue { get; set; }
        public string Value { get; set; }
    }

    public class invoiceCharge
    {
        public string totalCharged { get; set; }
        public string billingDate { get; set; }
        public string description { get; set; }
        public string chargeEntityId { get; set; }
        public string chargeEventTypeId { get; set; }
        public string customerTariffId { get; set; }
        public string notes { get; set; }
        public string flatRateAmount { get; set; }
        public string eventPerformedFrom { get; set; }
        public string chargeGlCode { get; set; }
        public string eventPerformedTo { get; set; }
        public string gkey { get; set; }
        public string extractGkey { get; set; }
        public string extractClass { get; set; }
        public string qtyDivisor { get; set; }
        public string quantity { get; set; }
        public string quantityBilled { get; set; }
        public string quantityUnit { get; set; }
        public string rateBilled { get; set; }
        public string isFlatRate { get; set; }
        public string rateMaxAmount { get; set; }
        public string rateMinAmount { get; set; }
        public string totalTaxes { get; set; }

        public chargeTariff chargeTariff { get; set; }

        public chargeTax chargeTax { get; set; }

        public invoiceCharge()
        {
            this.chargeTariff = new chargeTariff();
            this.chargeTariff.tariffRate = new tariffRate();
            this.chargeTax = new chargeTax();
        }

    }

    public class chargeTariff
    {
        public string description { get; set; }
        public string id { get; set; }
        public string longDescription { get; set; }
        public string glcode { get; set; }
        public tariffRate tariffRate { get; set; }

    }

    public class tariffRate
    {
        public string contractName { get; set; }
        public string currency { get; set; }
        public string effectiveDate { get; set; }
      
    }

    public class chargeTax
    {
        public string amount { get; set; }
        public string date { get; set; }
        public string rate { get; set; }
        public string taxItemId { get; set; }
    }
}
