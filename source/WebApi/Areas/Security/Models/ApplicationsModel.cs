using System.Collections.Generic;
using PaymentGateway.Model.PaymentGateway.Context;

namespace PaymentGateway.Areas.Security.Models
{
    public class ApplicationsModel
    {
        public RegisteredApplication RegisteredApplication { get; set; }
        public IEnumerable<RegisteredApplication> RegisteredApplications { get; set; }
    }
}