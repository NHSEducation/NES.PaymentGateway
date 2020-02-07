using System.Data.Entity;
using System.Linq;

using Ninject;
using PaymentGateway.Model;
using PaymentGateway.Model.PaymentGateway.Context;

namespace Service
{
    public class RegisterUserService : IRegisterUserService
    {
        private readonly IDbContext _dbContext;
        private readonly IDbSet<RegisteredUser> _registerUsers;

        #region constructors

        public RegisterUserService([Named("PaymentGateway")] IDbContext dbContext)
        {
            _dbContext = dbContext;
            _registerUsers = _dbContext.Set<RegisteredUser>();
        }

        #endregion

        #region methods

        public void Add(RegisteredUser registerUser)
        {
                _registerUsers.Add(registerUser);
                _dbContext.SaveChanges();
        }

        public int GetLoggedUserId(RegisteredUser registerUser)
        {
            var userId = (from user in _registerUsers
                where user.Username == registerUser.Username &&
                      user.Password == registerUser.Password
                select user.Id).FirstOrDefault();

            return userId;
        }

        public bool ValidateRegisteredUser(RegisteredUser registerUser)
        {
            var userCount = (from user in _registerUsers
                where user.Username == registerUser.Username &&
                      user.Password == registerUser.Password
                select user).Count();

            return userCount > 0;
        }

        public bool ValidateUsername(RegisteredUser registerUser)
        {
            var userCount = (from user in _registerUsers
                where user.Username == registerUser.Username
                select user).Count();

            return userCount > 0;
        }
        
        #endregion
    }
}
