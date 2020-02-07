using System;
using System.Web;
using System.Web.Http.Controllers;
using Common.Security;
using PaymentGateway.Common.Security;

namespace PaymentGateway.Util.Session
{
    public interface ISessionManager
    { 
            /// <summary>
            /// Initialises this instance (MVC).
            /// </summary>
            /// <param name="httpContext">The HTTP context.</param>
            void Initialise(HttpContextBase httpContext);

            /// <summary>
            /// Initialises this instance (Web API).
            /// </summary>
            /// <param name="httpContext"></param>
            void Initialise(HttpControllerContext httpContext);

            /// <summary>
            /// Gets the current user.
            /// </summary>
            /// <value>The current user.</value>
            User CurrentUser { get; }

            /// <summary>
            /// Gets/sets the users authentication status.
            /// </summary>
            bool IsAuthenticated { get; }

            /// <summary>
            /// Gets the current version.
            /// </summary>
            /// <value>The current version.</value>
            string CurrentVersion { get; }

            /// <summary>
            /// Called when user logs in.
            /// </summary>
            /// <param name="user">The user.</param>
            void OnLogin(User user);

            /// <summary>
            /// Called when [logout].
            /// </summary>
            void OnLogout();

            /// <summary>
            /// Called when a user impersonates another.
            /// </summary>
            /// <param name="user">The user.</param>
            void OnImpersonateUser(User user);

            /// <summary>
            /// Gets the current URL.
            /// </summary>
            Uri CurrentUrl { get; }

            /// <summary>
            /// Gets the current session id.
            /// </summary>
            string SessionToken { get; }
        }
    }