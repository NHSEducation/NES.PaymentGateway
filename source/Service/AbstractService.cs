using System.Data.Entity;
using System.Linq;

using PaymentGateway.Model;

namespace Service
{
    public abstract class AbstractService<T> : IService<T> where T : class, IEntity
    {
        #region public properties

        public IDbContext Context { get; set; }
        public IDbSet<T> Entities { get; set; }

        #endregion

        #region methods

        public void Delete(T entity)
        {
            Entities.Remove(entity);
            Context.SaveChanges();
        }

        public IQueryable<T> GetAll()
        {
            return Entities;
        }

        public T GetById(object id)
        {
            return Entities.Find(id);
        }

        public void SaveOrUpdate(T entity)
        {
            if (Entities.Find(entity.Id) == null)
            {
                Entities.Add(entity);
            }

            Context.SaveChanges();
        }

        public void Add(T entity)
        {
            if (Entities.Find(entity.Id) == null)
            {
                Entities.Add(entity);
            }
        }

        public void SetStateModified(T entity)
        {
            Context.Entry(entity).State = EntityState.Modified;
        }

        #endregion

    }
}
