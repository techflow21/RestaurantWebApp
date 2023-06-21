using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;


namespace Domain.Repository
{
    public interface IRepository<T> where T : class
    {
        /*Task<IEnumerable<T>> GetAll();
        Task<T> GetById(string id);
        Task<T> Add(T entity);
        Task<T> Update(T entity);
        Task Delete(T entity);*/


        Task<T> GetByName(string name);
        T Add(T obj);
        Task<T> AddAsync(T obj);
        void AddRange(IEnumerable<T> records);
        Task AddRangeAsync(IEnumerable<T> records);
        bool Any(Expression<Func<T, bool>> predicate = null);
        Task<bool> AnyAsync(Expression<Func<T, bool>> predicate = null);
        long Count(Expression<Func<T, bool>> predicate = null);
        Task<long> CountAsync(Expression<Func<T, bool>> predicate = null);
        bool Delete(T obj);
        bool Delete(Expression<Func<T, bool>> predicate);
        Task DeleteAsync(T obj);
        Task DeleteAsync(Expression<Func<T, bool>> predicate);
        bool DeleteById(object id);
        Task DeleteByIdAsync(object id);
        bool DeleteRange(Expression<Func<T, bool>> predicate);
        bool DeleteRange(IEnumerable<T> records);
        Task DeleteRangeAsync(IEnumerable<T> records);
        Task DeleteRangeAsync(Expression<Func<T, bool>> predicate);
        void Dispose();
        IEnumerable<T> GetAll(Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, params string[] includeProperties);
        Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>> filter = null, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, Func<IQueryable<T>, IIncludableQueryable<T, object>> include = null);
        IEnumerable<T> GetBy(Expression<Func<T, bool>> predicate = null, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, int? skip = null, int? take = null, params string[] includeProperties);
        Task<IEnumerable<T>> GetByAsync(Expression<Func<T, bool>> predicate = null, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, int? skip = null, int? take = null, params string[] includeProperties);
        Task<IEnumerable<T>> GetByAsync(Expression<Func<T, bool>> predicate = null, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, int? skip = null, int? take = null, Func<IQueryable<T>, IIncludableQueryable<T, object>> include = null);
        Task<T> GetSingleByAsync(Expression<Func<T, bool>> predicate = null, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, int? skip = null, int? take = null, Func<IQueryable<T>, IIncludableQueryable<T, object>> include = null, bool tracking = false);
        Task<T> GetSingleByAsync(Expression<Func<T, bool>> predicate);
        T GetById(object id);
        Task<T> GetByIdAsync(object id);
        IQueryable<T> GetQueryable(Expression<Func<T, bool>> predicate = null, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, int? skip = null, int? take = null, Func<IQueryable<T>, IIncludableQueryable<T, object>> include = null);
        T GetSingleBy(Expression<Func<T, bool>> predicate);

        Task<T> LastAsync(Expression<Func<T, bool>> predicate = null, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
            Func<IQueryable<T>, IIncludableQueryable<T, object>> include = null, bool disableTracking = true);

        int Save();
        Task<int> SaveAsync();
        Task<decimal> SumAsync(Expression<Func<T, decimal>> predicate);
        Task<int> SumAsync(Expression<Func<T, int>> predicate);
        Task<long> SumAsync(Expression<Func<T, long>> predicate);
        T Update(T obj);
        Task<T> UpdateAsync(T obj);
        void UpdateRange(IEnumerable<T> records);
        Task UpdateRangeAsync(IEnumerable<T> records);
    }
}
