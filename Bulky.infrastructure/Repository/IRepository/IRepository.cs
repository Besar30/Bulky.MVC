using System.Linq.Expressions;

namespace Bulky.infrastructure.Repository.IRepository
{
    public interface IRepository<T>
    {
        IEnumerable<T> GetAll();
        T Get(Expression<Func<T,bool>>filter);
        void Add(T entity);
        void Remove(T entity);
        void RemoveRange(IEnumerable<T> entities);
    }
}
