using System.Web.Mvc;
using PaymentGateway.Util.ActionFilters;

namespace PaymentGateway
{
    public class FilterConfig
    {
        #region MVC filtersd

        /// <summary>
        /// Register filters for MVC Controllers
        /// </summary>
        /// <param name="filters"></param>
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
            filters.Add(new LogActionFilter());
        }

        #endregion

        #region http filters

        /// <summary>
        /// Register global filters for ApiControllers
        /// </summary>
        /// <param name="filters"></param>
        public static void RegisterWebApiFilters(System.Web.Http.Filters.HttpFilterCollection filters)
        {
            // Enable Authorize attribute on all ApiController Actions
            //filters.Add(new System.Web.Http.AuthorizeAttribute());
            //filters.Add(new ApplicationApiAuthorizationAttribute());
            filters.Add(new LogApiActionFilter());
        }

        #endregion
    }
}
