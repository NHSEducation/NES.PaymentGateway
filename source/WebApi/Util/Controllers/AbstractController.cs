using System.Web.Mvc;
using System.Web.Routing;
using Common.Security;
using PaymentGateway.Areas;
using PaymentGateway.Util.Session;

namespace PaymentGateway.Util.Controllers
{
    public abstract partial class AbstractController : Controller
    {
        #region members

        // Anti-CSRF related.
        private readonly ValidateAntiForgeryTokenAttribute _validator;
        private readonly AcceptVerbsAttribute _verbs;

        #endregion

        #region properties

        /// <summary>
        /// Gets or sets the session manager
        /// </summary>
        //[Inject]
        public SessionManager SessionManager => SessionManager.Session;

        public BaseView BaseModel => new BaseView { SessionManager = SessionManager };

        #endregion

        #region constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="AbstractController"/> class.
        /// </summary>
        protected AbstractController()
        {
            _verbs = new AcceptVerbsAttribute(HttpVerbs.Get);
            _validator = new ValidateAntiForgeryTokenAttribute();
        }

        #endregion

        #region authentication helpers

        public User CurrentUser
        {
            get => SessionManager.CurrentUser;
            set => SessionManager.OnLogin(value);
        }

        public bool IsAuthenticated => SessionManager.IsAuthenticated;

        #endregion

        #region overrides

        protected override void Initialize(RequestContext requestContext)
        {
            // No client input will be checked on any controllers
            // Prevents HttpRequestValidationException in an MVC environment
            ValidateRequest = false;

            base.Initialize(requestContext);
            InitialiseSessionManager(requestContext);
        }

        #endregion

        #region protected methods

        protected void InitialiseSessionManager(RequestContext requestContext)
        {
            SessionManager.Initialise(requestContext.HttpContext);
        }

        #endregion
    }
}