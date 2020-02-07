using System.Collections.Generic;
using PaymentGateway.Model.PaymentGateway.Context;

namespace Service
{
    public interface IRegisteredUserService
    {
        void Add(RegisteredUser registeredUser);
        bool ValidateRegisteredUser(RegisteredUser registeredUser);
        bool ValidateUsername(RegisteredUser registeredUser);
        int GetLoggedUserId(RegisteredUser registeredUser);
        IEnumerable<RegisteredUser> GetAll();
        void Delete(int userId);
    }
}
