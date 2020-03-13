using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Common.Helpers;
using Ninject;
using PaymentGateway.Model;
using PaymentGateway.Model.PaymentGateway.Context;

namespace Service
{
    public class RegisteredUserService : IRegisteredUserService
    {
        private readonly IDbContext _dbContext;
        private readonly IDbSet<RegisteredUser> _registerUsers;

        #region constructors

        public RegisteredUserService([Named("PaymentGatewayDb")] IDbContext dbContext)
        {
            _dbContext = dbContext;
            _registerUsers = _dbContext.Set<RegisteredUser>();
        }

        #endregion

        #region methods

        public void Add(RegisteredUser user)
        {
                _registerUsers.Add(user);
                _dbContext.SaveChanges();
        }

        public void Delete(int userId)
        {
            var user = (from u in _registerUsers
                where u.Id == userId
                select u).FirstOrDefault();

            if (user == null) return;

            _dbContext.Entry(user).State = EntityState.Deleted;
            _dbContext.SaveChanges();
        }

        public IEnumerable<RegisteredUser> GetAll()
        {
            var users = (from u in _registerUsers where u.Username != "NesPGAdmin" select u).ToList();

            return users;
        }

        public int GetLoggedUserId(RegisteredUser registeredUser)
        {
            var encryptedPassword = EncryptionHelper.Encrypt(registeredUser.Password);
            var userId = (from user in _registerUsers
                where user.Username == registeredUser.Username &&
                      user.Password == encryptedPassword
                          select user.Id).FirstOrDefault();

            return userId;
        }

        public bool ValidateRegisteredUser(RegisteredUser registeredUser)
        {
            var encryptedPassword = EncryptionHelper.Encrypt(registeredUser.Password);
            var userCount = (from user in _registerUsers
                where user.Username == registeredUser.Username &&
                      user.Password == encryptedPassword
                             select user).Count();

            return userCount > 0;
        }

        public bool ValidateUsername(RegisteredUser registeredUser)
        {
            var userCount = (from user in _registerUsers
                where user.Username == registeredUser.Username
                select user).Count();

            return userCount > 0;
        }

        #endregion
    }
}
