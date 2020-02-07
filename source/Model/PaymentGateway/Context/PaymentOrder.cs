using System;

namespace PaymentGateway.Model.PaymentGateway.Context
{
    public class PaymentOrder : IEntity
    {
        public PaymentOrder()
        {
        }

        public int Id { get; set; }
        public string VendorTxCode { get; set; }
        public int BookingId { get; set; }
        public double Amount { get; set; }
        public string CostCentre { get; set; }
        public string AccountCode { get; set; }
        public string ProjectCode { get; set; }
        public string VPSTxID { get; set; }
        public string Status { get; set; }
        public string StatusDetail { get; set; }
        public DateTime ProcessedDate { get; set; }
    }
}
