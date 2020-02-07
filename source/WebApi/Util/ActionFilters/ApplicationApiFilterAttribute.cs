using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using Common.Security.Activities;
using PaymentGateway.Model.PaymentGateway.Context;
using Service;

namespace PaymentGateway.Util.ActionFilters
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
    public class ApplicationApiFilterAttribute : ActionFilterAttribute
    {
        #region properties

        public static IActivityType Activity { get; set; }

        #endregion

        #region constructors

        public ApplicationApiFilterAttribute()
        {
        }

        public ApplicationApiFilterAttribute(Type activity)
        {
            if ((activity).BaseType != typeof(ActivityType))
            {
                throw new ArgumentException("activity");
            }

            Activity = (ActivityType)Activator.CreateInstance(activity);

        }

        #endregion

        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            var queryString = actionContext.Request.GetQueryNameValuePairs().ToDictionary(x => x.Key, x => x.Value);

            var contains = queryString.Any(h => h.Key.Contains("name"));

            if (!contains)
                actionContext.Response = new HttpResponseMessage(HttpStatusCode.Forbidden);
            else
            {
                var application = queryString["name"];
                using (var context = new PGContext())
                {
                    var appService = new RegisteredApplicationService(context);

                    var registeredApplications = appService.GetAllApplications();

                    var exists = registeredApplications.Any(a => a.Name == application);

                    if (!exists)
                    {
                        actionContext.Response = new HttpResponseMessage(HttpStatusCode.Forbidden);
                    }
                }
            }
            base.OnActionExecuting(actionContext);
        }       
    }
}