using System.Linq.Expressions;

namespace Application.Interfaces.Repositories.EFCore
{
    public interface IGenericRepository<T> where T : class
    {
        T Add(T entity);

        IEnumerable<T> AddRange(IEnumerable<T> entities);

        T Update(T entity);

        IQueryable<T> AsQueryable();

        IEnumerable<T> UpdateRange(IEnumerable<T> entities);

        void Delete(T entity);

        void DeleteRange(IEnumerable<T> entities);

        Task DeleteRangeAsync(IEnumerable<T> entities);

        //Task<T> GetByIdAsync(int id);
        IQueryable<T> GetAll();

        Task<IReadOnlyList<T>> GetAllAsync();

        Task<IReadOnlyList<T>> GetPagedResponseAsync(int pageNumber, int pageSize);

        Task<T> AddAsync(T entity);

        Task<IEnumerable<T>> AddRangeAsync(IEnumerable<T> entities);

        Task UpdateAsync(T entity);

        Task DeleteAsync(T entity);

        IQueryable<T> Query(Expression<Func<T, bool>> filter);

        Task<bool> AnyAsync(Expression<Func<T, bool>> filter);

        Task<T> FirstOrDefaultAsync(Expression<Func<T, bool>> filter);

        IQueryable<T> FindByIncludes(Expression<Func<T, bool>> predicate,
            params Expression<Func<T, object>>[] includes);

        T FirstOrDefault(Expression<Func<T, bool>> filter);

        T LastOrDefault<TKey>(Expression<Func<T, TKey>> keySelector);

        int Count();

        int Count(Expression<Func<T, bool>> predicate);
    }
}