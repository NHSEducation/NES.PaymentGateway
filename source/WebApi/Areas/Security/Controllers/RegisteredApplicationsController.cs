using System;
using System.Web.Mvc;
using PaymentGateway.Areas.Security.Models;
using PaymentGateway.Model.PaymentGateway.Context;
using PaymentGateway.Util.ActionFilters;
using PaymentGateway.Util.Controllers;
using PaymentGateway.Util.Helpers;
using Service;

namespace PaymentGateway.Areas.Security.Controllers
{
    [CustomAuthorizationFilter]
    public class RegisteredApplicationsController : AbstractController
    {
        #region members

        private readonly IRegisteredApplicationService _registerApplicationService;

        #endregion

        #region constructors

        public RegisteredApplicationsController()
        {
        }

        public RegisteredApplicationsController(IRegisteredApplicationService registerApplicationService)
        {
            _registerApplicationService = registerApplicationService;
        }

        #endregion

        #region actions

        // GET: RegisterApplication/Create
        public ActionResult CreateApplication()
        {
            var applicationsModel = new ApplicationsModel();
            var registeredApplications = _registerApplicationService.GetAllApplications();
            applicationsModel.RegisteredApplications = registeredApplications;
            applicationsModel.RegisteredApplication = new RegisteredApplication{Created = DateTime.Now};

            ViewBag.PackageVersion = ConfigurationHelper.PackageVersion;

            return View(applicationsModel);
        }

        // POST: RegisterApplication/Create
        [HttpPost]
        public ActionResult CreateApplication(ApplicationsModel applicationsModel)
        {
            try
            {
                var registeredApplication = applicationsModel.RegisteredApplication;

                if (!ModelState.IsValid)
                {
                    return View("CreateApplication", applicationsModel);
                }

                // Validating Username
                if (_registerApplicationService.ValidateApplication(registeredApplication))
                {
                    ModelState.AddModelError("", "Application is Already Registered");
                    return View("CreateApplication", applicationsModel);
                }


                registeredApplication.Created = DateTime.Now;

                _registerApplicationService.Add(registeredApplication);

                var registeredApplications = _registerApplicationService.GetAllApplications();
                applicationsModel.RegisteredApplications = registeredApplications;
                applicationsModel.RegisteredApplication = new RegisteredApplication { Created = DateTime.Now };

                TempData["ApplicationMessage"] = "Application Registered Successfully";
                ModelState.Clear();

                ViewBag.PackageVersion = ConfigurationHelper.PackageVersion;

                return View("CreateApplication", applicationsModel);
            }
            catch
            {
                return View();
            }
        }

        [HttpPost]
        public ActionResult DeleteApplication(int applicationId)
        {
            _registerApplicationService.Delete(applicationId);
            TempData["ApplicationMessage"] = "Application Deleted";
            ModelState.Clear();

            var applicationsModel = new ApplicationsModel();
            var registeredApplications = _registerApplicationService.GetAllApplications();
            applicationsModel.RegisteredApplications = registeredApplications;
            applicationsModel.RegisteredApplication = new RegisteredApplication { Created = DateTime.Now };
            return View("CreateApplication", applicationsModel);
        }

        #endregion
    }
}
