using System.Data.Entity;
using System.Linq;
using PaymentGateway.Model;

namespace Service
{
    public interface IService<T> where T : class
    {
        IDbContext Context { get; set; }
        IDbSet<T> Entities { get; set; }

        void Add(T entity);
        void Delete(T entity);
        IQueryable<T> GetAll();
        T GetById(object id);

        void SaveOrUpdate(T entity);
        void SetStateModified(T entity);
    }
}
