using System;
using System.Collections.Generic;

namespace PaymentGateway.Model.PaymentGateway.Common
{
    [Serializable]
    public class Basket
    {
        public string AgentId { get; set; }
        public List<BasketItem> Items { get; set; }
        public decimal DeliveryNetAmount { get; set; }
        public decimal DeliveryTaxAmount { get; set; }
        public decimal DeliveryGrossAmount { get; set; }
        public List<Discount> Discounts { get; set; }
        public string ShipId { get; set; }
        public string ShippingMethod { get; set; }
        public string ShippingFaxNo { get; set; }
    }
}
