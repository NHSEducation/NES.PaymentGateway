using System;
using System.Reflection;
using System.Web;
using System.Web.Http.Controllers;
using Common.Security;
using PaymentGateway.Common.Security;
using PaymentGateway.Util.Handlers;

namespace PaymentGateway.Util.Session
{
    public class SessionManager //: ISessionManager
    {
        #region members

        private const string SESSION_KEY_USER = "NESKey_User";
        private const string SESSION_KEY_AUTHENTICATED = "NESKey_Authenticated";
        private User _currentUser;
        private bool _isAuthenticated;
        private static SessionManager session;

        #endregion

        #region properties

        public static SessionManager Session => session ?? (session = new SessionManager());

        /// <summary>
        /// Gets the current user.
        /// </summary>
        /// <value>The current user.</value>
        public User CurrentUser
        {
            get => RetrieveCurrentUser();
            private set => SaveCurrentUser(value);
        }

        public bool IsAuthenticated
        {
            get => RetrieveAuthentication();
            private set => SaveAuthenticated(value);
        }

        /// <summary>
        /// Gets the current version.
        /// </summary>
        /// <value>The current version.</value>
        public string CurrentVersion => $"{Assembly.GetExecutingAssembly().GetName().Version}";

        /// <summary>
        /// Gets the current URL.
        /// </summary>
        /// <value>The current URL.</value>
        public Uri CurrentUrl => HttpContext.Request.Url;

        /// <summary>
        /// Gets the current session id.
        /// </summary>
        public string SessionToken => RSAClass.Encrypt(HttpContext.Session.SessionID);

        /// <summary>
        /// Gets or sets the HTTP context (MVC).
        /// </summary>
        /// <value>The HTTP context.</value>
        private HttpContextBase HttpContext { get; set; }

        /// <summary>
        /// Gets or sets the HTTP Controller Context (Wec API).
        /// </summary>
        private HttpControllerContext HttpControllerContext { get; set; }

        #endregion

        #region constructors

        private SessionManager()
        {
        }

        #endregion

        public void Initialise(HttpContextBase context)
        {
            HttpContext = context;
        }

        public void Initialise(HttpControllerContext context)
        {
            HttpControllerContext = context;
        }


        public void OnLogin(User user)
        {
            //_logger.DebugFormat("SessionManager logging in user {0}", user == null ? "<NULL>" : user.Username);

            CurrentUser = user;
        }

        public void OnLogout()
        {
            //_logger.Debug("SessionManager logging out");

            CurrentUser = null;
            if (HttpContext != null)
            {
                if (HttpContext.Session != null) HttpContext.Session.Abandon();
            }
        }

        public void OnImpersonateUser(User user)
        {
            ConfigureUser(user);
        }

        #region helpers

        #region active directory helpers

        private User RetrieveUserFromActiveDirectory()
        {
            // Create a User object from the Active Directory information
            User activeDirectoryUser = User.CreateFromDomainUsername(HttpContext.User.Identity.GetLogin());
            if (activeDirectoryUser != null)
            {
                ConfigureUser(activeDirectoryUser);

                SaveAuthenticated(true);
            }
            else
            {
                SaveAuthenticated(false);
            }

            return activeDirectoryUser;
        }

        private void ConfigureUser(User targetUser)
        {
            // TODO : This is only required for testing.
            //if (targetUser.PersonalReference.IsNullOrWhiteSpace())
            //{
            //    if (User.IsDeveloperUser(targetUser))
            //    {
            //        targetUser.SetPropertyValue(o => o.PersonalReference, User.TestDeveloperPersonalReference);
            //    }
            //}

            //// Set additional elements of the User object
            //if (targetUser.Reportees.Count == 0)
            //{
            //    SetAdditionalUserData(targetUser);
            //}

            // Decide on the Role that this User should have...
            //if (targetUser.Role == User.Roles.Standard)
            //{
            //    if (ModelContext.Config.Security.AdminGroups.Split(',').ToList().Intersect(targetUser.UserGroups).Any())
            //    {
            //        targetUser.SetRole(User.Roles.Admin);
            //    }
            //    else if (ModelContext.Config.Security.AdminUsers.ToLower().Split(',').ToList().Contains(targetUser.Username.ToLower()))
            //    {
            //        targetUser.SetRole(User.Roles.Admin);
            //    }

            //}

            SaveCurrentUser(targetUser);
        }


        #endregion

        #region session helpers

        private User RetrieveUserFromSession()
        {
            return HttpContext.Session[SESSION_KEY_USER] as User;
        }

        private void SaveUserToSession(User user)
        {
            HttpContext.Session[SESSION_KEY_USER] = user;
        }

        private void SaveAuthenticationToSession(bool authenticated)
        {
            HttpContext.Session[SESSION_KEY_AUTHENTICATED] = authenticated;
        }

        #endregion

        #region user helpers

        private bool RetrieveAuthentication()
        {
            return true;
        }

        private void SaveAuthenticated(bool authenticated)
        {
            SaveAuthenticationToSession(authenticated);
        }
        /// <summary>
        /// Get User's details either from session or from ActiveDirectory
        /// </summary>
        /// <returns></returns>
        private User RetrieveCurrentUser()
        {
            if (HttpContext != null)
            {
                if (HttpContext.Request.IsAuthenticated)
                {
                    if (_currentUser == null)
                    {
                        _currentUser = RetrieveUserFromSession();
                    }
                    if (_currentUser == null)
                    {
                        _currentUser = RetrieveUserFromActiveDirectory();
                    }
                }
            }
            else if (HttpControllerContext != null)
            {
                //if(HttpControllerContext.Request.Headers.Authorization.
            }

            return _currentUser;
        }

        private void SaveCurrentUser(User user)
        {
            _currentUser = user;
            SaveUserToSession(_currentUser);
        }

        #endregion

        #endregion
    }
}