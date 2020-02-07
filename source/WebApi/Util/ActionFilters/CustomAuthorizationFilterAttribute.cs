using System;
using System.Web.Mvc;
using System.Web.Routing;
using Common.Security.Activities;


namespace PaymentGateway.Util.ActionFilters
{
    public class CustomAuthorizationFilterAttribute : ActionFilterAttribute
    {
        #region properties

        public static IActivityType Activity { get; set; }

        #endregion

        #region constructors

        public CustomAuthorizationFilterAttribute()
        {
        }

        public CustomAuthorizationFilterAttribute(Type activity)
        {
            if ((activity).BaseType != typeof(ActivityType))
            {
                throw new ArgumentException("activity");
            }

            Activity = (ActivityType) Activator.CreateInstance(activity);

        }

        #endregion

        public override void OnActionExecuting(ActionExecutingContext actionContext)
        {
            if (actionContext.HttpContext.Session["loggedOn"] == null)
            {
                //actionContext.Result = new RedirectToRouteResult(new RouteValueDictionary {{ "Controller", "Home" },
                //    { "Action", "Login" } });

                var contr = actionContext.Controller;

                var urlHelper = new UrlHelper(actionContext.RequestContext);
                var redirectUrl = urlHelper.Action("Login", "Home", new { area = "" });
                actionContext.Result = new RedirectResult(redirectUrl);
                actionContext.Result.ExecuteResult(contr.ControllerContext);
            }
        }
    }
}