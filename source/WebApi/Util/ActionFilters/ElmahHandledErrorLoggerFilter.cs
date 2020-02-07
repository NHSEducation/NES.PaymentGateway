using System.Web.Http.Filters;

namespace PaymentGateway.Util.ActionFilters
{
    public class ElmahHandledErrorLoggerFilter : ExceptionFilterAttribute
    {
        public override void OnException(HttpActionExecutedContext actionExecutedContext)
        {
            if (actionExecutedContext.Exception != null)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(actionExecutedContext.Exception);
            }

            base.OnException(actionExecutedContext);
        }
    }
}