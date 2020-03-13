using System;

namespace PaymentGateway.Model.PaymentGateway.Common
{
    [Serializable]
    public class Discount
    {
        public string Fixed { get; set; }
        public string Description { get; set; }
    }
}
