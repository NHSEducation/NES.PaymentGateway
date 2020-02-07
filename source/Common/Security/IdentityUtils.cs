using System.Security.Principal;

namespace PaymentGateway.Common.Security
{
    /// <summary>
    /// Extension methods for IIdentity
    /// </summary>
    public static class IdentityUtils
    {
        /// <summary>
        /// Gets the domain element of the Identity.
        /// </summary>
        /// <param name="identity">The identity.</param>
        public static string GetDomain(this IIdentity identity)
        {
            string s = identity.Name;
            int stop = s.IndexOf("\\");
            return (stop > -1) ? s.Substring(0, stop) : string.Empty;
        }

        /// <summary>
        /// Gets the login element of the Identity.
        /// </summary>
        /// <param name="identity">The identity.</param>
        public static string GetLogin(this IIdentity identity)
        {
            string s = identity.Name;
            int stop = s.IndexOf("\\");
            return (stop > -1) ? s.Substring(stop + 1, s.Length - stop - 1) : string.Empty;
        }
    }
}
