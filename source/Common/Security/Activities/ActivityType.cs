using System;
using System.Collections.Generic;
using System.Linq;
using Common.Helpers;


namespace Common.Security.Activities
{
    public abstract class ActivityType : IActivityType
    {
        #region Authorisation

        /// <summary>
        /// Determines whether the specified user is authorised for the activity.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <returns>
        /// 	<c>true</c> if the specified user is authorised; otherwise, <c>false</c>.
        /// </returns>
        public virtual bool IsAuthorised(User user)
        {
            var roles = new List<String>(string.Join(",", RequiredRoles.Select(r => EnumHelper.GetDescription(r.GetType(), r))).Split(','));
            return (RequiredRoles.Count == 0) || (user.UserGroups.Intersect(roles).Any());
        }

        /// <summary>
        /// Gets the required roles.
        /// </summary>
        /// <value>The required roles.</value>
        public abstract IList<AuthorisedRoles> RequiredRoles { get; }

        #endregion

        #region Initialisation

        /// <summary>
        /// Initialises this instance.
        /// </summary>
        /// <param name="activity"> </param>
        /// <returns></returns>
        public ActivityType Initialise(ActivityType activity)
        {
            return this;
        }

        #endregion
    }
}
