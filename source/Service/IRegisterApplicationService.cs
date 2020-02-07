using System.Collections.Generic;
using PaymentGateway.Model.PaymentGateway.Context;

namespace Service
{
    public interface IRegisterApplicationService
    {
        void Add(RegisteredApplication registerApplication);
        IEnumerable<RegisteredApplication> GetAllApplications();
        bool ValidateApplication(RegisteredApplication registerApplication);
        RegisteredApplication UpdateStatus(RegisteredApplication registerApplication);
        void GenerateClientKey(out string clientId, out string clientSecret);
    }
}
