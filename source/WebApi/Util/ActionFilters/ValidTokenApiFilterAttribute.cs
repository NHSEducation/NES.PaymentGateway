using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using Common.Helpers;
using Common.Security.Activities;
using log4net.Util.TypeConverters;
using Newtonsoft.Json;
using PaymentGateway.Model.PaymentGateway.Context;
using Service;

namespace PaymentGateway.Util.ActionFilters
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
    public class ValidTokenApiFilterAttribute : ActionFilterAttribute
    {
        #region properties

        public static IActivityType Activity { get; set; }

        #endregion

        #region constructors

        public ValidTokenApiFilterAttribute()
        {
        }

        public ValidTokenApiFilterAttribute(Type activity)
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
            var request = actionContext.Request;
            var headers = request.Headers;
            var contains = headers.Any(h => h.Key.Contains("X-AuthToken"));

            if (!contains)
                actionContext.Response = new HttpResponseMessage(HttpStatusCode.Forbidden);
            else
            {
                var headerValues = headers.GetValues("X-AuthToken");
                var jsonInfo = headerValues.FirstOrDefault();

                var authToken = JsonConvert.DeserializeObject<string>(jsonInfo);

                var decryptedToken = EncryptionHelper.Decrypt(authToken);

                var tokenArray = decryptedToken.Split('~');
                var applicationName = tokenArray[0];
                var timeString = tokenArray[1];

                // check if token is too old.
                if (Convert.ToDateTime(timeString).AddSeconds(60) < DateTime.Now)
                {
                    actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.Unauthorized, "The Token has expired!"); 
                }

                // check that the application is registered
                using (var context = new PGContext())
                {
                    var appService = new RegisteredApplicationService(context);

                    var registeredApplications = appService.GetAllApplications();

                    var exists = registeredApplications.Any(a => a.Name == applicationName);

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