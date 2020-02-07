using System;
using System.Web.Mvc;
using Common.Helpers;
using PaymentGateway.Areas.Security.Models;
using PaymentGateway.Model.PaymentGateway.Context;
using PaymentGateway.Util.ActionFilters;
using PaymentGateway.Util.Controllers;
using Service;

namespace PaymentGateway.Areas.Security.Controllers
{
    [CustomAuthorizationFilter]
    public class RegisteredUsersController : AbstractController
    {
        #region members

        private readonly IRegisteredUserService _registerUserService;

        #endregion

        #region constructors

        public RegisteredUsersController()
        {
        }

        public RegisteredUsersController(IRegisteredUserService registerUserService)
        {
            _registerUserService = registerUserService;
        }

        #endregion

        public ActionResult Index()
        {
            return View();
        }

        // GET: RegisterUser/Create
        public ActionResult CreateUser()
        {
            var registeredUsers = _registerUserService.GetAll();
            var model = new UserModel() {RegisteredUser = new RegisteredUser() { Created = DateTime.Now } , RegisteredUsers = registeredUsers};

            return View(model);
        }

        // POST: RegisterUser/Create
        [HttpPost]
        public ActionResult CreateUser(UserModel userModel)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View("CreateUser", userModel);
                }

                var registeredUser = userModel.RegisteredUser;

                // Validating Username 
                if (_registerUserService.ValidateUsername(registeredUser))
                {
                    ModelState.AddModelError("", "User is Already Registered");
                    var users = _registerUserService.GetAll();
                    userModel.RegisteredUsers = users;
                    return View("CreateUser", userModel);
                }
                registeredUser.Created = DateTime.Now;

                // Encrypting Password with AES 256 Algorithm
                registeredUser.Password = EncryptionHelper.Encrypt(registeredUser.Password);

                // Saving User Details in Database
                _registerUserService.Add(registeredUser);
                TempData["UserMessage"] = "User Registered Successfully";
                ModelState.Clear();

                var registeredUsers = _registerUserService.GetAll();
                var model = new UserModel() { RegisteredUser = new RegisteredUser() { Created = DateTime.Now }, RegisteredUsers = registeredUsers };

                return View("CreateUser", model);
            }
            catch
            {
                return View();
            }
        }

        [HttpPost]
        public ActionResult DeleteUser(int userId)
        {
            _registerUserService.Delete(userId);
            TempData["UserMessage"] = "User Deleted";
            ModelState.Clear();

            var registeredUsers = _registerUserService.GetAll();
            var model = new UserModel() { RegisteredUser = new RegisteredUser() { Created = DateTime.Now }, RegisteredUsers = registeredUsers };
            return View("CreateUser", model);
        }

    }
}