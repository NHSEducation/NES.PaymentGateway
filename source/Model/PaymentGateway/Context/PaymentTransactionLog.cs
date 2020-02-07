using System;

namespace PaymentGateway.Model.PaymentGateway.Context
{
    public class PaymentTransactionLog : IEntity
    {
        public int Id { get; set; }
        public string VendorTxCode { get; set; }
        public decimal? Amount { get; set; }
        public string VPSTxID { get; set; }
        public string RegistrationStatus { get; set; }
        public string RegistrationStatusDetail { get; set; }
        public DateTime? RegistrationTime { get; set; }
        public string SecurityKey { get; set; }
        public string AuthorisationStatus { get; set; }
        public string AuthorisationStatusDetail { get; set; }
        public DateTime? AuthorisationTime { get; set; }
        public string CardType { get; set; }
        public string LastFourDigits { get; set; }
    }
}