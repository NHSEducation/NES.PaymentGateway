using Ninject.Modules;
using PaymentGateway.Model;
using PaymentGateway.Model.PaymentGateway.Context;
using Service;

namespace PaymentGateway.Util.Ninject
{
    public class ServicesModule : NinjectModule
    {
        public override void Load()
        {
            Bind<IDbContext>().ToConstructor(ctx => new PGContext()).Named("PaymentGatewayDb");
            Bind(typeof(IService<>)).To(typeof(GatewayService<>));
            Bind(typeof(ISageService)).To(typeof(SageService));
        }
    }
}