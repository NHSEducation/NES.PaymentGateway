namespace PaymentGateway.Model.PaymentGateway.Common
{
    public class AddressDetails
    {
        /// <summary>
        /// Limit: 20 chars
        /// </summary>
        public string Surname { get; set; }

        /// <summary>
        /// Limit: 20 chars
        /// </summary>
        public string Firstnames { get; set; }

        /// <summary>
        /// Limit: 100 chars
        /// </summary>
        public string Address1 { get; set; }

        /// <summary>
        /// Limit: 100 chars
        /// </summary>
        public string Address2 { get; set; }

        /// <summary>
        /// Limit: 40 chars
        /// </summary>
        public string City { get; set; }

        /// <summary>
        /// Limit:10 chars
        /// </summary>
        public string PostCode { get; set; }

        /// <summary>
        /// 2 chars = GB
        /// </summary>
        public string Country { get; set; }

        /// <summary>
        /// blank
        /// </summary>
        public string State { get; set; }

        /// <summary>
        /// Limit: 20 chars
        /// </summary>
        public string Phone { get; set; }
    }
}
