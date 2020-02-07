using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Common.Helpers;
using Ninject;
using PaymentGateway.Model;
using PaymentGateway.Model.PaymentGateway.Context;

namespace Service
{
    public class RegisteredApplicationService : IRegisteredApplicationService
    {
        private readonly IDbContext _dbContext;
        private readonly IDbSet<RegisteredApplication> _registeredApplications;

        #region constructors

        public RegisteredApplicationService([Named("PaymentGateway")] IDbContext dbContext)
        {
            _dbContext = dbContext;
            _registeredApplications = _dbContext.Set<RegisteredApplication>();
        }

        #endregion

        #region methods

        public void Add(RegisteredApplication registerApplication)
        {
            try
            {
                _registeredApplications.Add(registerApplication);
                _dbContext.SaveChanges();
            }
            catch (Exception e)
            {
                var test = 1;
            }
        }

        public void Delete(int applicationId)
        {
            var application = (from a in _registeredApplications
                        where a.Id == applicationId
                        select a).FirstOrDefault();

            if (application == null) return;

            _dbContext.Entry(application).State = EntityState.Deleted;
            _dbContext.SaveChanges();
        }

        public IEnumerable<RegisteredApplication> GetAllApplications()
        {
            return (from app in _registeredApplications select app).ToList();
        }

        public bool ValidateApplication(RegisteredApplication registerApplication)
        {
            var applicationCount = (from app in _registeredApplications
                                    where app.Name == registerApplication.Name
                select app).Count();

            return applicationCount > 0;
        }

        public RegisteredApplication UpdateStatus(RegisteredApplication registeredApplication)
        {
            var application = (from app in _registeredApplications
                    where app.Id == registeredApplication.Id
                    select app)
                .FirstOrDefault();

            application.IsActive = registeredApplication.IsActive;
            _dbContext.SaveChanges();

            return application;
        }

        #endregion
    }
}
