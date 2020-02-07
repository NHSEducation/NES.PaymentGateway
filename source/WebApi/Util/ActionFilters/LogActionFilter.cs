using System.Text;
using System.Web.Mvc;
using PaymentGateway.Util.Services;

namespace PaymentGateway.Util.ActionFilters
{
    public class LogActionFilter : ActionFilterAttribute
    {
        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            var sessionId = filterContext.HttpContext.Session.SessionID;
            var actionDescriptor = filterContext.ActionDescriptor;
            var controllerName = actionDescriptor.ControllerDescriptor.ControllerName;
            var actionName = actionDescriptor.ActionName;
            var userName = filterContext.HttpContext.User.Identity.Name;
            var address = filterContext.HttpContext.Request.UserHostAddress;

            var routeId = string.Empty;

            if (filterContext.RouteData.Values["id"] != null)
            {
                routeId = filterContext.RouteData.Values["id"].ToString();
            }

            var message = new StringBuilder();

            if (!string.IsNullOrEmpty(routeId))
            {
                message.Append("RouteId=");
                message.Append(routeId);
            }

            var logService = new LoggingService();
            logService.Log(sessionId, userName, address, controllerName, actionName, message.ToString());

            base.OnActionExecuted(filterContext);
        }
    }
}