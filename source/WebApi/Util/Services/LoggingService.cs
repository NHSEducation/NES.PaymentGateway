using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Reflection;
using log4net;

namespace PaymentGateway.Util.Services
{
    public class LoggingService : ILoggingService
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public void Log(string message)
        {
            log.Info(message);
        }
        public void Log(string sessionId, string userName, string ipAddress, string controllerName, string actionName, string message)
        {
            log4net.MDC.Set("session", sessionId);
            log4net.MDC.Set("user", userName);
            log4net.MDC.Set("address", ipAddress);
            log4net.MDC.Set("controller", controllerName);
            log4net.MDC.Set("action", actionName);

            Log(message);
        }
    }

    public interface ILoggingService
    {
        void Log(string message);
        void Log(string sessionId, string userName, string ipAddress, string controllerName, string actionName, string message);
    }
}