using System.Data.Entity;
using System.Data.Entity.Infrastructure;

namespace PaymentGateway.Model
{
    public interface IDbContext
    {
        int SaveChanges();

        IDbSet<TEntity> Set<TEntity>() where TEntity : class;

        DbEntityEntry<TEntity> Entry<TEntity>(TEntity entity) where TEntity : class;
        void Dispose();
    }
}
