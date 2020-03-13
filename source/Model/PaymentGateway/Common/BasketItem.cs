using System;

namespace PaymentGateway.Model.PaymentGateway.Common
{
    [Serializable]
    public class BasketItem
    {
        public string Description { get; set; }
        public string ProductSku { get; set; }
        public string ProductCode { get; set; }
        public string Quantity { get; set; } 
        public string UnitNetAmount { get; set; } 
        public string UnitTaxAmount { get; set; } 
        public string UnitGrossAmount { get; set; }
        public string TotalGrossAmount { get; set; }
        public string RecipientFName { get; set; }
        public string RecipientLName { get; set; }
        public string RecipientMName { get; set; } 
        public string RecipientSal { get; set; } 
        public string RecipientEmail { get; set; }
        public string RecipientPhone { get; set; }
        public string RecipientAdd1 { get; set; }
        public string RecipientAdd2 { get; set; }
        public string RecipientCity { get; set; }
        public string RecipientState { get; set; }
        public string RecipientCountry { get; set; }
        public string RecipientPostCode { get; set; }
        public string ItemShipNo { get; set; }
        public string ItemGiftMsg { get; set; }
    }
}
