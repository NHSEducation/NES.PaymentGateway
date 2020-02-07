using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Mvc;
using Common.Security;
using Ninject;
using PaymentGateway.Common.Security;
using PaymentGateway.Util.ActionFilters;
using PaymentGateway.Util.Session;

namespace PaymentGateway.Util.Controllers
{
    public abstract partial class AbstractApiController : ApiController
    {
        #region members

        // Anti-CSRF related.
        private readonly ValidateAntiForgeryTokenAttribute _validator;

        #endregion

        #region properties

        /// <summary>
        /// Gets or sets the session manager
        /// </summary>
        //[Inject]
        public SessionManager SessionManager
        {
            get { return SessionManager.Session; }
        }

        #endregion

        #region constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="AbstractController"/> class.
        /// </summary>
        protected AbstractApiController()
        {

            _validator = new ValidateAntiForgeryTokenAttribute();
        }

        #endregion

        #region authentication helpers

        public User CurrentUser
        {
            get { return SessionManager.CurrentUser; }
            set { SessionManager.OnLogin(value); }
        }

        public bool IsAuthenticated
        {
            get { return SessionManager.IsAuthenticated; }
        }

        #endregion

        #region overrides

        protected override void Initialize(HttpControllerContext controllerContext)
        {
            base.Initialize(controllerContext);
            InitialiseSessionManager(controllerContext);
        }

        #endregion

        #region protected methods

        protected void InitialiseSessionManager(HttpControllerContext requestContext)
        {
            SessionManager.Initialise(requestContext);
        }

        #endregion
    }
}