using Domain.Generic;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace Infrastructure.Data.EntityFrameworkCoreSqlServer.Repositories.Generic
{
    abstract class BaseRepository<T, R> : IRepository<T, R> where T : BaseEntity<R> where R : BaseDto
    {
        protected readonly DbContext DbContext;
        protected readonly DbSet<T> DbSet;

        internal BaseRepository(DbContext dbContext)
        {
            DbContext = dbContext;
            DbSet = dbContext.Set<T>();
        }

        public virtual async Task<T> Add(T entity, CancellationToken cancellationToken = default)
        {
            entity = (await DbSet.AddAsync(entity, cancellationToken)).Entity;

            var now = DateTime.UtcNow;
            DbContext.Entry(entity).Property(entity => entity.CreatedAt).CurrentValue = now;
            DbContext.Entry(entity).Property(entity => entity.UpdatedAt).CurrentValue = now;

            return entity;
        }

        public virtual Task<T> Find(int id, CancellationToken cancellationToken = default)
        {
            IQueryable<T> queryable = DbSet;

            queryable = queryable.Where(t => t.Id == id);
            
            return First(queryable, cancellationToken);
        }

        public virtual Task<T> FindOneBy(
            Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
        {
            IQueryable<T> queryable = DbSet;

            queryable = queryable.Where(predicate);
            
            return First(queryable, cancellationToken);
        }

        public virtual void Update(T entity)
        {
            if (DbContext.Entry(entity).State == EntityState.Detached)
            {
                DbSet.Attach(entity);
            }

            DbContext.Entry(entity).Property(entity => entity.UpdatedAt).CurrentValue = DateTime.UtcNow;
            DbContext.Entry(entity).State = EntityState.Modified;
        }

        public virtual void Delete(T entity)
        {
            if (DbContext.Entry(entity).State == EntityState.Detached)
            {
                DbSet.Attach(entity);
            }

            DbSet.Remove(entity);
        }

        public virtual Task<long> Count(
            Expression<Func<T, bool>> predicate = null,
            CancellationToken cancellationToken = default)
        {
            if (predicate != null)
            {
                return DbSet.LongCountAsync(predicate, cancellationToken);
            }

            return DbSet.LongCountAsync(cancellationToken);
        }

        public virtual Task<List<R>> GetAll(
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
            Expression<Func<T, bool>> predicate = null,
            CancellationToken cancellationToken = default)
        {
            IQueryable<T> queryable = DbSet;
            queryable = (predicate != null) ? queryable.Where(predicate) : queryable;
            queryable = (orderBy != null) ? orderBy(queryable) : queryable;

            var dtoQuery = queryable.Select(t => t.ToDto()).AsNoTracking();

            return dtoQuery.ToListAsync(cancellationToken);
        }

        public virtual Task<PaginatedList<R>> FilterBy(
          string search,
          int page = 1,
          int perPage = 15,
          Expression<Func<T, bool>> predicate = null,
          CancellationToken cancellationToken = default)
        {
            IQueryable<T> queryable = DbSet;

            if (predicate != null) { queryable = queryable.Where(predicate); }

            if (!string.IsNullOrEmpty(search)) { queryable = DoSearch(queryable, search); }

            var dtoQueryable = queryable.Select(t => t.ToDto()).AsNoTracking();

            return Paginator<R>.PaginateAsync(dtoQueryable, page, perPage, cancellationToken);
        }

        public virtual async Task<bool> IsEmpty(CancellationToken cancellationToken = default)
        {
            return ! await DbSet.AnyAsync(cancellationToken);
        }
       
        protected async Task<T> First(
            IQueryable<T> queryable, 
            CancellationToken cancellationToken = default)
        {
            T entity = null;
            try
            {
                entity = await queryable.FirstAsync(cancellationToken);
            }
            catch (InvalidOperationException) { }

            return entity;
        }

        protected abstract IQueryable<T> DoSearch(IQueryable<T> queryable, string search);
    }
}
