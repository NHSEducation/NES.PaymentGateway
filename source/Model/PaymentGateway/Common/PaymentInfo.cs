namespace PaymentGateway.Model.PaymentGateway.Common
{
    public class PaymentInfo
    {
        public string VendorTxCode { get; set; }
        public string TotalBookingFee { get; set; }
        public string AccountCode { get; set; }
        public int BookingId { get; set; }
        public string CostCentre { get; set; }
        public string ProjectCode { get; set; }
        public string NotificationUrl { get; set; }
        public AddressDetails BillingDetails { get; set; }
        public AddressDetails DeliveryDetails { get; set; }
        public string CustomerEmail { get; set; }
        public string Basket { get; set; }
        public bool IsMobile { get; set; }
    }
}
