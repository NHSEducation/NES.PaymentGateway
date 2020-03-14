using System.Linq;
using System.Web.Mvc;
using PaymentGateway.Model.PaymentGateway.Context;
using PaymentGateway.Util.ActionFilters;
using PaymentGateway.Util.Controllers;
using PaymentGateway.Util.Helpers;
using Service;


namespace PaymentGateway.Controllers
{    
    public class HomeController :  AbstractController
    {
        #region members

        private readonly IRegisteredUserService _registeredUserService;

        #endregion

        #region constructors

        public HomeController()
        {
        }

        public HomeController(IRegisteredUserService registeredUserService)
        {
            _registeredUserService = registeredUserService;
        }

        #endregion

        [HttpGet]
        [CustomAuthorizationFilter]
        public ActionResult Index()
        {
            ViewBag.Title = "Home Page";
            ViewBag.PackageVersion = ConfigurationHelper.PackageVersion;

            return View();
        }

        [HttpGet]
        public ActionResult Login()
        {
            ViewBag.PackageVersion = ConfigurationHelper.PackageVersion;

            return View();
        }

        [HttpPost]
        public ActionResult Login(RegisteredUser user)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors);

            if (ModelState.IsValid)
            {
                // check that the user is registered
                var isRegistered = _registeredUserService.ValidateRegisteredUser(user);

                if (isRegistered)
                {
                    Session["loggedOn"] = user.Username;
                    return RedirectToAction("Index");
                }
            }

            ViewBag.PackageVersion = ConfigurationHelper.PackageVersion;

            return View(user);
        }
    }
}
