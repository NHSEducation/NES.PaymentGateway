using System.Web.Mvc;
using PaymentGateway.Model.PaymentGateway.Context;
using PaymentGateway.Util.ActionFilters;
using PaymentGateway.Util.Controllers;
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

            return View();
        }

        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(RegisteredUser user)
        {
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

            return View(user);
        }
    }
}
