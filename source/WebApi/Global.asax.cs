using System;
using System.Data.Entity;
using System.Reflection;
using System.Web;
using System.Web.Http;
using System.Web.Http.Dispatcher;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using log4net;

using PaymentGateway.Common.Security;
using PaymentGateway.Model.PaymentGateway.Context;
using PaymentGateway.Util.ActionFilters;
using PaymentGateway.Util.Areas;
using PaymentGateway.Util.Handlers;

namespace PaymentGateway
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        #region private members

        private ILog _logger;

        #endregion

        #region application events

        protected void Application_PreSendRequestHeaders(Object sender, EventArgs e)
        {
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
        }

        protected void Application_Start()
        {
            BootstrapLog4net();
            InitialiseLogger(true);
            _logger.Info("Web application start.");


            AreaRegistration.RegisterAllAreas();

            WebApiConfig.Register(GlobalConfiguration.Configuration);

            // mvc global filters
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            // http global filters
            FilterConfig.RegisterWebApiFilters(GlobalConfiguration.Configuration.Filters);

            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            //BundleMobileConfig.RegisterBundles(BundleTable.Bundles);

            // Create the database contexts for release build (live databases - DO NOT SEED)
            InitialiseDatabaseContext();

            // if using development databases, then seed them with initial data
            // ************* CHECK WEB.CONFIG DOES NOT POINT TO LIVE DB's
            //SeedDevelopmentDatabases();


            //GlobalConfiguration.Configuration.MessageHandlers.Add(new TokenValidationHandler());
            GlobalConfiguration.Configuration.Services.Replace(typeof(IHttpControllerSelector), new AreaHttpControllerSelector(GlobalConfiguration.Configuration));
            GlobalConfiguration.Configuration.Filters.Add(new ElmahHandledErrorLoggerFilter());


            GlobalConfiguration.Configuration.EnsureInitialized();
        }

        protected void Application_End(object sender, EventArgs e)
        {
            InitialiseLogger(false);
            _logger.Info("Web application end.");
            LogReasonForApplicationEnd();
        }

        protected void Application_Error(Object sender, EventArgs e)
        {
            // Log error.
            var ex = Server.GetLastError();
            InitialiseLogger(false);
            _logger.Error(ex.ToString());
        }

        #endregion

        #region session

        protected void Session_Start(Object sender, EventArgs e)
        {
            InitialiseLogger(false);
            _logger.InfoFormat("Session <{0}> start on IP address <{1}>.", this.Session.SessionID, this.Request.UserHostAddress);

            //var user = HttpContext.Current.User.Identity.GetLogin();
            //SetLogContext(this.Session.SessionID, user);
        }

        protected void Session_End(Object sender, EventArgs e)
        {
            InitialiseLogger(false);
            _logger.InfoFormat("Session <{0}> end.", this.Session.SessionID);
        }

        #endregion

        #region log4net

        private static void BootstrapLog4net()
        {
            log4net.Config.XmlConfigurator.Configure();
        }

        private void InitialiseLogger(bool forceInit)
        {
            if (forceInit || (_logger == null))
            {
                _logger = log4net.LogManager.GetLogger(typeof(WebApiApplication));
            }
        }

        private void SetLogContext(string sessionId, string userName)
        {
            log4net.MDC.Set("session", sessionId);
            log4net.MDC.Set("user", userName);
        }

        private void LogReasonForApplicationEnd()
        {
            var httpRuntime = (HttpRuntime)typeof(HttpRuntime).InvokeMember("_theRuntime",
                                                                                        BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.GetField,
                                                                                        null,
                                                                                        null,
                                                                                        null);
            if (httpRuntime != null)
            {
                var shutDownMessage = (string)httpRuntime.GetType().InvokeMember("_shutDownMessage",
                                                                                        BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.GetField,
                                                                                        null,
                                                                                        httpRuntime,
                                                                                        null);

                var shutDownStack = (string)httpRuntime.GetType().InvokeMember("_shutDownStack",
                                                                                    BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.GetField,
                                                                                    null,
                                                                                    httpRuntime,
                                                                                    null);

                _logger.InfoFormat("ShutDown Message={0}", shutDownMessage);
                _logger.InfoFormat("ShutDown Stack={0}", shutDownStack);
            }
        }

        #endregion

        #region database

        private static void InitialiseDatabaseContext()
        {
            // PaymentGateway database
            Database.SetInitializer<PGContext>(null);

        }

        #endregion


    }
}
