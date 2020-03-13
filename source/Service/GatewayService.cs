using Ninject;
using PaymentGateway.Model;

namespace Service
{
    public class GatewayService<T> : AbstractService<T> where T : class, IEntity
    {
        #region constructors

        public GatewayService([Named("PaymentGatewayDb")]IDbContext context)
        {
            Context = context;
            Entities = context.Set<T>();
        }

        #endregion
    }
}
