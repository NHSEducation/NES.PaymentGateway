using System.Collections.Generic;
using PaymentGateway.Model.PaymentGateway.Context;

namespace Service
{
    public interface IRegisteredApplicationService
    {
        void Add(RegisteredApplication registerApplication);
        void Delete(int applicationId);
        IEnumerable<RegisteredApplication> GetAllApplications();
        bool ValidateApplication(RegisteredApplication registerApplication);
        RegisteredApplication UpdateStatus(RegisteredApplication registerApplication);
    }
}
