using Domain.Core.Abstracts;
using Domain.Generic.Auth.Abstracts;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Infrastructure.Data.Abstracts
{
    public interface IUnitOfWork
    {
        ITodoItemRepository TodoItemRepository { get; }
        IUserRepository UserRepository { get; }

        /// <summary>
        /// Persists changes on entities to data source
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task Commit(CancellationToken cancellationToken = default);

        /// <summary>
        /// Executes the specified action with a single transaction          
        /// </summary>
        void Transactional(Action<IUnitOfWork> action);
    }
}
