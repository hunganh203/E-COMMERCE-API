using Application.Interfaces.Repositories.EFCore;
using EfCore.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

#nullable disable

namespace EFCore.Persistence.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        protected readonly ApplicationDbContext _dbContext;

        public GenericRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public T Add(T entity)
        {
            _dbContext.Set<T>().Add(entity);
            return entity;
        }

        public IEnumerable<T> AddRange(IEnumerable<T> entities)
        {
            _dbContext.Set<T>().AddRange(entities);
            return entities;
        }

        public T Update(T entity)
        {
            _dbContext.Entry(entity).State = EntityState.Modified;
            //_dbContext.Set<T>().Update(entity);
            return entity;
        }

        public IEnumerable<T> UpdateRange(IEnumerable<T> entities)
        {
            foreach (var entity in entities)
            {
                _dbContext.Entry(entity).State = EntityState.Modified;
            }
            return entities;
        }

        public void Delete(T entity)
        {
            _dbContext.Set<T>().Remove(entity);
        }

        public IQueryable<T> GetAll()
        {
            return _dbContext
                .Set<T>()
                .AsNoTracking();
        }

        public IQueryable<T> AsQueryable()
        {
            return _dbContext
                .Set<T>()
                .AsQueryable();
        }

        public async Task<IReadOnlyList<T>> GetAllAsync()
        {
            return await _dbContext
                 .Set<T>()
                 .AsNoTracking()
                 .ToListAsync();
        }

        //public async Task<T> GetByIdAsync(int id)
        //{
        //    return await _dbContext.Set<T>().FindAsync(id);
        //}

        public async Task<IReadOnlyList<T>> GetPagedResponseAsync(int pageNumber, int pageSize)
        {
            return await _dbContext
                .Set<T>()
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<T> AddAsync(T entity)
        {
            await _dbContext.Set<T>().AddAsync(entity);
            await _dbContext.SaveChangesAsync();
            return entity;
        }

        public async Task<IEnumerable<T>> AddRangeAsync(IEnumerable<T> entities)
        {
            await _dbContext.Set<T>().AddRangeAsync(entities);
            await _dbContext.SaveChangesAsync();
            return entities;
        }

        public async Task UpdateAsync(T entity)
        {
            _dbContext.Entry(entity).State = EntityState.Modified;
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteAsync(T entity)
        {
            _dbContext.Set<T>().Remove(entity);
            await _dbContext.SaveChangesAsync();
        }

        public void DeleteRange(IEnumerable<T> entities)
        {
            _dbContext.Set<T>().RemoveRange(entities);
        }

        public async Task DeleteRangeAsync(IEnumerable<T> entities)
        {
            _dbContext.Set<T>().RemoveRange(entities);
            await _dbContext.SaveChangesAsync();
        }

        public IQueryable<T> Query(Expression<Func<T, bool>> filter)
        {
            return _dbContext.Set<T>()
                .Where(filter)
                .AsNoTracking();
        }

        public T FirstOrDefault(Expression<Func<T, bool>> filter)
        {
            return _dbContext.Set<T>().AsNoTracking().FirstOrDefault(filter);
        }

        public Task<T> FirstOrDefaultAsync(Expression<Func<T, bool>> filter)
        {
            return _dbContext.Set<T>().AsNoTracking().FirstOrDefaultAsync(filter);
        }

        public IQueryable<T> FindByIncludes(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includes)
        {
            var query = GetAll().Where(predicate);
            return includes.Aggregate(query, (current, includeProperty) => current.Include(includeProperty));
        }

        public Task<bool> AnyAsync(Expression<Func<T, bool>> filter)
        {
            return _dbContext.Set<T>().AnyAsync(filter);
        }

        public T LastOrDefault<TKey>(Expression<Func<T, TKey>> keySelector)
        {
            return _dbContext.Set<T>().OrderBy(keySelector).LastOrDefault();
        }

        public int Count()
        {
            return _dbContext.Set<T>().Count();
        }

        public int Count(Expression<Func<T, bool>> predicate)
        {
            return _dbContext.Set<T>().Count(predicate);
        }
    }
}