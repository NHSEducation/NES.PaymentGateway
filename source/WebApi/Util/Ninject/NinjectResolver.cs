using System.Web.Http.Dependencies;
using Ninject;

namespace PaymentGateway.Util.Ninject
{
    public class NinjectResolver : NinjectScope, IDependencyResolver
    {
        private IKernel _kernel;

        #region constructors

        public NinjectResolver(IKernel kernel) : base(kernel)
        {
            _kernel = kernel;
        }

        #endregion

        #region methods

        public IDependencyScope BeginScope()
        {
            return new NinjectScope(_kernel.BeginBlock());
        }

        #endregion
    }
}