using PaymentGateway.Model.PaymentGateway.Context;

namespace Service
{
    public interface IRegisterUserService
    {
        void Add(RegisteredUser registerUser);
        bool ValidateRegisteredUser(RegisteredUser registerUser);
        bool ValidateUsername(RegisteredUser registerUser);
        int GetLoggedUserId(RegisteredUser registerUser);
    }
}
