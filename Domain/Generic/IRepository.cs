using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace Domain.Generic
{
    public interface IRepository<T, R> where T : BaseEntity<R> where R : BaseDto
    {
        /// <summary>
        /// Adds a new entity, sets necessary timestamps for the entity and initializes tracking for it
        /// </summary>
        /// <param name="entity">New entity to add</param>
        /// <param name="cancellationToken"></param>
        /// <returns>Returns entity added</returns>
        Task<T> Add(T entity, CancellationToken cancellationToken = default);

        /// <summary>
        /// Updates an entity and sets necessary timestamps for it
        /// </summary>
        /// <param name="id"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>Returns entity of type T whose Id property value is equal to id. Returns null if not found</returns>
        Task<T> Find(int id, CancellationToken cancellationToken = default);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>Returns first entity of type T that satisfies the predicate. Returns null if not found</returns>
        Task<T> FindOneBy(
            Expression<Func<T, bool>> predicate, 
            CancellationToken cancellationToken = default);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entity">Entity of type T to update</param>
        void Update(T entity);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entity">Entity of type T to delete</param>
        void Delete(T entity);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>The total number of entities of type T that satisfy the predicate</returns>
        Task<long> Count(
            Expression<Func<T, bool>> predicate, 
            CancellationToken cancellationToken = default);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="orderBy"></param>
        /// <param name="predicate"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>A list of the corresponding Dto of type R for every Entity of type T that satisfies the predicate. 
        /// If no predicate is specified, all corresponding Dtos of the entities are retuned.
        /// </returns>
        Task<List<R>> GetAll(
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
            Expression<Func<T, bool>> predicate = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="search"></param>
        /// <param name="page"></param>
        /// <param name="perPage"></param>
        /// <param name="predicate"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>List containing dto of all entities that satisfy the predicate</returns>
        Task<PaginatedList<R>> FilterBy(
            string search = null,
            int page = 1,
            int perPage = 15,
            Expression<Func<T, bool>> predicate = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Check if repository is empty
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns>True if repository is empty, false otherwise</returns>
        Task<bool> IsEmpty(CancellationToken cancellationToken = default);
    }
}
