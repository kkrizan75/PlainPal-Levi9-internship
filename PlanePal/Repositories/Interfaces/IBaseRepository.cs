using System.Linq.Expressions;

namespace PlanePal.Repositories.Interfaces
{
    public interface IBaseRepository<T, TKey>
    {
        int Update(T entity);

        void Delete(T entity);

        void Add(T entity);

        Task<int> SaveChanges();

        Task<int> Create(T entity);

        Task<int> UpdateAndSave(T entity);

        T GetOne(TKey key);

        Task<IEnumerable<T>> GetAll();

        Task<IEnumerable<T>> GetAllBy(Expression<Func<T, bool>> predicate);
    }
}