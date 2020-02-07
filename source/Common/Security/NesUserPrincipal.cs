using System;
using System.DirectoryServices.AccountManagement;
using System.Threading;


namespace Common.Security
{
    /// <summary>
    /// A Principal Extension to model the Nes Active Directory User
    /// and the extended properties it makes available.
    /// </summary>
    [DirectoryRdnPrefix("CN")]
    [DirectoryObjectClass("user")]
    public class NesUserPrincipal : UserPrincipal
    {
        #region constructors

        public NesUserPrincipal(PrincipalContext context) : base(context)
        {
        }

        public NesUserPrincipal(PrincipalContext context, string samAccountName, string password, bool enabled) : base(context, samAccountName, password, enabled)
        {
        }

        #endregion

        #region FindByIdentity overloads

        /// <summary>
        /// Type specific overload of FindByIdentity
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="identityValue">The identity value.</param>
        /// <returns></returns>
        public new static NesUserPrincipal FindByIdentity(PrincipalContext context, string identityValue)
        {
            return (NesUserPrincipal)FindByIdentityWithType(context,
                                                             typeof(NesUserPrincipal),
                                                             identityValue);
        }

        /// <summary>
        /// Type specific overload of FindByIdentity
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="identityType">Type of the identity.</param>
        /// <param name="identityValue">The identity value.</param>
        /// <returns></returns>
        public static NesUserPrincipal FindByIdentity(PrincipalContext context,
                                                          IdentityType identityType,
                                                          string identityValue, int retryNumber = 0)
        {
            try
            {
                return (NesUserPrincipal)FindByIdentityWithType(context,
                                                                 typeof(NesUserPrincipal),
                                                                 identityType,
                                                                 identityValue);
            }
            catch (AppDomainUnloadedException)
            {
                if (retryNumber > 3)
                {
                    throw;
                }
                retryNumber += 1;
                Thread.Sleep(500);
                return FindByIdentity(context, identityType, identityValue, retryNumber);
            }
        }

        #endregion

        #region helpers

        /// <summary>
        /// Gets the authorization groups with retry.
        /// </summary>
        /// <param name="retryNumber">The retry number.</param>
        /// <remarks>
        /// http://stackoverflow.com/questions/5895128/attempted-to-access-an-unloaded-appdomain-when-using-system-directoryservices
        /// </remarks>
        public PrincipalSearchResult<Principal> GetAuthorizationGroupsWithRetry(int retryNumber = 0)
        {
            try
            {
                return GetAuthorizationGroups();
            }
            catch (AppDomainUnloadedException)
            {
                if (retryNumber > 3)
                {
                    throw;
                }
                retryNumber += 1;
                Thread.Sleep(500);
                return GetAuthorizationGroupsWithRetry(retryNumber);
            }
        }

        #endregion
    }
}
