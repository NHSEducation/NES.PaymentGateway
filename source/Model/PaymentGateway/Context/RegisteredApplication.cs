using System;

namespace PaymentGateway.Model.PaymentGateway.Context
{
    public class RegisteredApplication : IEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime Created { get; set; }
        public bool IsActive { get; set; }
    }
}
