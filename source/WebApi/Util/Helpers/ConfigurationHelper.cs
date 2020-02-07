using System;
using System.Configuration;
using System.Web;
using Ninject.Activation;

namespace PaymentGateway.Util.Helpers
{
    public static class ConfigurationHelper
    {
        #region properties

        public static string NotificationUrl => GetValue("NotificationUrl");
        public static string SageEnvironment => GetValue("SageEnvironment");

        #endregion

        #region Methods

        /// <summary>
        /// Gets the AppSetting value for a particular key and also throws exception if it is not found
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string GetValue(string key)
        {
            var value = ConfigurationManager.AppSettings[key];


            if (string.IsNullOrEmpty(value))
            {
                throw new Exception("Failed to load AppSetting value for this key:" + key);
            }
            else
            {
                return value;
            }
        }

        /// <summary>
        /// Gets the ConnectionString value for a particular key and also throws exception if it is not found
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string GetConnectionString(string key)
        {
            var value = ConfigurationManager.ConnectionStrings[key];

            if (value == null)
            {
                throw new Exception("Failed to load ConfigurationString value for this key:" + key);
            }
            else
            {
                return value.ConnectionString;
            }
        }

        /// <summary>
        /// Get the root Url for the application
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static string GetRootUrl(HttpRequestBase request)
        {
            return request.Url.GetLeftPart(UriPartial.Authority);
        }

        #endregion
    }
}
