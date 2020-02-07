using System;
using System.Web;
using System.Web.Http;
using Microsoft.Web.Infrastructure.DynamicModuleHelper;
using Ninject;
using Ninject.Extensions.Conventions;
using Ninject.Web.Common;
using Ninject.Web.Common.WebHost;
using PaymentGateway;
using PaymentGateway.Model;
using PaymentGateway.Model.PaymentGateway.Context;
using PaymentGateway.Util.Ninject;
using Service;

[assembly: WebActivatorEx.PreApplicationStartMethod(typeof(NinjectWebCommon), "Start")]
[assembly: WebActivatorEx.ApplicationShutdownMethodAttribute(typeof(NinjectWebCommon), "Stop")]

namespace PaymentGateway
{
    public static class NinjectWebCommon
    {
        #region members

        private static readonly Bootstrapper Bootstrapper = new Bootstrapper();

        #endregion

        #region private methods

        //private static IEnumerable<Type> Excluding => new List<Type>{typeof(HelpController) };

        /// <summary>
        /// Load you modules or register you services here!
        /// </summary>
        /// <param name="kernel"></param>
        private static void RegisterServices(IKernel kernel)
        {

            kernel.Bind(x => x.FromAssembliesMatching("*")
                     .SelectAllClasses()
                     //.Excluding(Excluding)
                     .BindDefaultInterface());


            kernel.Bind(typeof(IService<>)).To(typeof(GatewayService<>)).Named("PaymentGateway");
            kernel.Bind<IDbContext>().ToConstructor(ctx => new PGContext()).Named("PaymentGateway");


            GlobalConfiguration.Configuration.DependencyResolver = new NinjectResolver(kernel);
        }

        #region kernel

        /// <summary>
        /// Creates the kernel and will manage your application
        /// </summary>
        /// <returns></returns>
        private static IKernel CreateKernel()
        {
            var kernel = new StandardKernel();
            kernel.Bind<Func<IKernel>>().ToMethod(ctx => () => new Bootstrapper().Kernel);
            kernel.Bind<IHttpModule>().To<HttpApplicationInitializationHttpModule>();

            RegisterServices(kernel);
            return kernel;
        }

        #endregion

        #endregion

        #region public methods

        /// <summary>
        /// Starts the application
        /// </summary>
        public static void Start()
        {
            DynamicModuleUtility.RegisterModule(typeof(OnePerRequestHttpModule));
            DynamicModuleUtility.RegisterModule(typeof(NinjectHttpModule));
            Bootstrapper.Initialize(CreateKernel);
        }

        /// <summary>
        /// Stops the application
        /// </summary>
        public static void Stop()
        {
            Bootstrapper.ShutDown();
        }

        #endregion
    }

}