using Microsoft.EntityFrameworkCore;
using PlanePal.DbContext;
using PlanePal.Repositories.Interfaces;
using System.Linq.Expressions;

namespace PlanePal.Repositories.Implementation
{
    public class BaseRepository<T, TKey> : IBaseRepository<T, TKey> where T : class
    {
        protected readonly PlanePalDbContext _dataContext;

        protected BaseRepository(PlanePalDbContext dataContext)
        {
            _dataContext = dataContext;
        }

        public void Update(T entity)
        {
            _dataContext.ChangeTracker.Clear();
            _dataContext.Set<T>().Update(entity);
        }

        public void Delete(T entity)
        {
            _dataContext.Set<T>().Remove(entity);
        }

        public void Add(T entity)
        {
            _dataContext.Set<T>().Add(entity);
        }

        public async Task<int> SaveChanges()
        {
            return await _dataContext.SaveChangesAsync();
        }

        public async Task<int> UpdateAndSave(T entity)
        {
            Update(entity);
            return await SaveChanges();
        }

        public T GetOne(TKey key)
        {
            return _dataContext.Set<T>().Find(key);
        }

        public async Task<IEnumerable<T>> GetAll()
        {
            return await _dataContext.Set<T>().ToListAsync();
        }

        public async Task<int> Create(T entity)
        {
            Add(entity);
            return await SaveChanges();
        }

        int IBaseRepository<T, TKey>.Update(T entity)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<T>> GetAllBy(Expression<Func<T, bool>> predicate)
        {
            IEnumerable<T> entities = await _dataContext.Set<T>().Where(predicate).ToListAsync();
            return entities;
        }
    }
}