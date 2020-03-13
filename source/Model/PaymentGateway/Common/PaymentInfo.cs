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

        /// <summary>
        /// Only use if BasketXml is NOT used
        /// </summary>
        public string Basket { get; set; }

        /// <summary>
        /// Only use if Basket is NOT used
        /// </summary>
        public Basket BasketXml { get; set; }

        /// <summary>
        /// This can be used to supply information on the customer for purposes such as fraud screening
        /// </summary>
        public string CustomerXml { get; set; }

        /// <summary>
        /// Use this field to override current surcharge settings in “My Sage Pay” for the current transaction.
        /// Percentage and fixed amount surcharges can be set for different payment types
        /// </summary>
        public string SurchargeXml { get; set; }

        /// <summary>
        /// Use this field to pass any data you wish to be displayed against the transaction in MySagePay
        /// </summary>
        public string VendorData { get; set; }
        public bool IsMobile { get; set; }
    }
}
