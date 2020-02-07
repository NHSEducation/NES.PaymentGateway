using System.Collections.Generic;
using PaymentGateway.Model.PaymentGateway.Context;

namespace PaymentGateway.Areas.Security.Models
{
    public class UserModel
    {
        public RegisteredUser RegisteredUser { get; set; }
        public IEnumerable<RegisteredUser> RegisteredUsers { get; set; }
    }
}