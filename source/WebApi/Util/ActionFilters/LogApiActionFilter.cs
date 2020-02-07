using System.Linq;
using System.ServiceModel.Channels;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

using PaymentGateway.Util.Services;

namespace PaymentGateway.Util.ActionFilters
{
    public class LogApiActionFilter : ActionFilterAttribute
    {
        public override void  OnActionExecuting(HttpActionContext actionContext)
        {
            //var user = actionContext.CurrentUser();

            var args = actionContext.ActionArguments;
            var param = args.ElementAt(0);

            var paramName = param.Key;
            var data = new JavaScriptSerializer().Serialize(param.Value);

            var controllerName = actionContext.ActionDescriptor.ControllerDescriptor.ControllerName;
            var actionName = actionContext.ActionDescriptor.ActionName;

            string address;
            if (actionContext.Request.Properties.ContainsKey("MS_HttpContext"))
            {
                address = ((HttpContextWrapper)actionContext.Request.Properties["MS_HttpContext"]).Request.UserHostAddress;
            }
            else if (actionContext.Request.Properties.ContainsKey(RemoteEndpointMessageProperty.Name))
            {
                var prop = (RemoteEndpointMessageProperty)actionContext.Request.Properties[RemoteEndpointMessageProperty.Name];
                address = prop.Address;
            }
            else
            {
                address = "";
            }

            var message = string.Format("paramName: {0}, value: {1}", paramName, data);

            var logService = new LoggingService();
            logService.Log(null, null, address, controllerName, actionName, message);

            base.OnActionExecuting(actionContext);
        }
    }
}