using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using System.Security.Principal;
using Common.Security.Activities;
using PaymentGateway.Common.Security;

namespace Common.Security
{
    /// <summary>
    /// Models a system User (information pulled from Active Directory)
    /// </summary>
    [Serializable]
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public class User
    {
        #region enums

        /// <summary>
        /// Represents the different statuses of users within the system
        /// </summary>
        public enum DisabledStatuses
        {
            Active,
            Disabled
        }

        #endregion

        #region members

        private string _username;
        private string _personalReference;
        private string _misId;
        private string _forename;
        private string _surname;
        private string _email;
        private DisabledStatuses _disabledStatus;
        private AuthorisedRoles _role;
        private bool _impersonated;
        private string _trueUsername;
        private IList<string> _userGroups;

        #endregion

        #region properties

        /// <summary>
        /// Gets the role for the user.
        /// </summary>
        public virtual AuthorisedRoles Role { get { return _role; } protected set { _role = value; } }

        /// <summary>
        /// Gets the user groups for the user.
        /// </summary>
        public virtual IList<string> UserGroups { get { return _userGroups; } protected set { _userGroups = value; } }

        /// <summary>
        /// Gets / Sets the username for the the user
        /// </summary>
        public virtual string Username
        {
            get { return _username; }
            protected set { _username = value; }
        }

        #endregion

        #region constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="User"/> class.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <param name="misId"> </param>
        /// <param name="forename">The forename.</param>
        /// <param name="surname">The surname.</param>
        /// <param name="email">The email.</param>
        /// <param name="impersonated">if set to <c>true</c> [impersonated].</param>
        /// <param name="trueUsername">The true username.</param>
        /// <param name="disabledStatus">The disabledStatus.</param>
        /// <param name="role">The role.</param>
        /// <param name="userGroups">The user groups.</param>
        public User(string username,
                    string misId,
                    string forename,
                    string surname,
                    string email,
                    bool impersonated,
                    string trueUsername,
                    DisabledStatuses disabledStatus,
                    AuthorisedRoles role,
                    IList<string> userGroups)
        {
            _username = username;
            _misId = misId;
            _forename = forename;
            _surname = surname;
            _email = email;
            _impersonated = impersonated;
            _trueUsername = trueUsername;
            _disabledStatus = disabledStatus;
            _role = role;
            _userGroups = userGroups;
        }

        #endregion

        #region active directory groups

        public virtual bool HasAnyRole(IPrincipal user, params string[] roles)
        {
            return roles.Any(user.IsInRole);
        }

        #endregion

        #region activity

        /// <summary>
        /// Determines whether the specified command is authorised.
        /// </summary>
        /// <param name="activityType"> </param>
        /// <returns>
        /// 	<c>true</c> if the specified command is authorised; otherwise, <c>false</c>.
        /// </returns>
        public virtual bool IsAuthorisedForActivity(ActivityType activityType)
        {
            return activityType.IsAuthorised(this);
        }

        #endregion

        #region helpers

        /// <summary>
        /// Creates a User object based on information pulled 
        /// from Active Directory for the specified username
        /// </summary>
        public static User CreateFromCurrentDomainUser()
        {
            return CreateFromDomainUsername(WindowsIdentity.GetCurrent().GetLogin());
        }

        /// <summary>
        /// Creates a User object based on information pulled 
        /// from Active Directory for the specified username
        /// </summary>
        public static User CreateFromDomainUsername(string userLoginName,
                                                    string trueUsername = null,
                                                    AuthorisedRoles? requestedRole = null,
                                                    string reporteePersonalReference = "")
        {
            User createdUser = null;

            try
            {

                // Get the Principal Context (effectively the Active Directory Server)
                PrincipalContext domainPrincipalContext = GetPrincipalContext();

                // Find the specified user
                NesUserPrincipal userPrincipal = NesUserPrincipal.FindByIdentity(domainPrincipalContext,
                                                                                 IdentityType.SamAccountName,
                                                                                 userLoginName);

                // If found - grab its details
                if (userPrincipal != null)
                {
                    // Get the "standard" properties
                    string username = userPrincipal.SamAccountName;  // this will be same as userLoginName, but with correct "casing" - so "smitha" -> "SmithA"
                    string surname = userPrincipal.Surname;
                    string firstname = userPrincipal.GivenName;
                    string email = userPrincipal.EmailAddress;

                    // Question the value of this - if the user account isn't enabled,
                    // they shouldn't be able to access the intranet and, therefore, 
                    // request this page?!
                    bool enabled = userPrincipal.Enabled.GetValueOrDefault(false);


                    string misId = userPrincipal.EmployeeId;                    // eg "5BF81"

                    // Unused at present, but available
                    //string displayName = userPrincipal.ToString();              // eg "Smith, Albert"
                    //string telephone = userPrincipal.VoiceTelephoneNumber;      // eg  "+44 1505 356123"
                    //string postDescription = userPrincipal.Description;         // eg "D IT SNR APPLICATIONS DEVELOPER/1"

                    // Now the groups that this user is a member of.
                    // In testing we sometimes get an App Unloaded exception
                    // so this method does a silent re-try
                    PrincipalSearchResult<Principal> groups = userPrincipal.GetAuthorizationGroupsWithRetry();
                    IList<string> groupList = groups.Select(o => o.Name).ToList();

                    // Create the user using the information we've found.
                    createdUser = new User(username,
                                           misId,
                                           firstname,
                                           surname,
                                           email,
                                           trueUsername != null,
                                           trueUsername,
                                           enabled
                                               ? DisabledStatuses.Active
                                               : DisabledStatuses.Disabled,
                                           requestedRole ?? AuthorisedRoles.Default,
                                           groupList);
                }
            }
            #region Test Code For Non-Active Directory Development
            catch (PrincipalServerDownException)
            {
                // TODO : IW : remove - only required for local development
                createdUser = new User(WindowsIdentity.GetCurrent().GetLogin(),
                    "",
                    "Gordon",
                    "Paton",
                    "patong@strathclydefire.org",
                    true,
                    "patong",
                    DisabledStatuses.Active,
                    AuthorisedRoles.Default,
                    new List<string>() { "ggMOBS Full", "ggMOBS Restricted" });
            }
            #endregion
            catch
            {
                throw;
            }


            return createdUser;
        }

        /// <summary>
        /// Gets the principal context.
        /// </summary>
        /// <returns></returns>
        public static PrincipalContext GetPrincipalContext()
        {
            PrincipalContext domainPrincipalContext = null;

            // Establish domain context
            try
            {
                // Try a basic connection (asking for any Active Directory Domain Server)
                domainPrincipalContext = new PrincipalContext(ContextType.Domain);
            }
            catch
            {
                throw;
            }

            return (domainPrincipalContext);
        }

        #endregion
    }
}
