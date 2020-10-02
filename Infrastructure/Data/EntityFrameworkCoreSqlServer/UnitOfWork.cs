using Domain.Core.Abstracts;
using Domain.Generic.Auth.Abstracts;
using Infrastructure.Data.Abstracts;
using Infrastructure.Data.EntityFrameworkCoreSqlServer.Repositories.Core;
using Infrastructure.Utils.Abstracts;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;

namespace Infrastructure.Data.EntityFrameworkCoreSqlServer
{
    class UnitOfWork : IUnitOfWork
    {
        readonly DbContext DbContext;
        readonly IRandomStringGenerator RandomStringGenerator;

        ITodoItemRepository todoItemRepository;
        public ITodoItemRepository TodoItemRepository
        {
            get
            {
                // Lazy initialization
                // Note that synchronization is not needed since the lifetime of the UOW in IOC container will be scoped (i.e a unique instance per request)
                // Also the uow for a request won't be used in multiple threads at the same time
                return todoItemRepository ??= new TodoItemRepository(DbContext, RandomStringGenerator);
            }
        }

        IUserRepository userRepository;
        public IUserRepository UserRepository
        {
            get
            {               
                return userRepository ??= new UserRepository(DbContext);
            }
        }

        public UnitOfWork(
            DbContext dbContext, 
            IRandomStringGenerator randomStringGenerator)
        {
            DbContext = dbContext;
            RandomStringGenerator = randomStringGenerator;
        }

        public async Task Commit(CancellationToken cancellationToken = default)
        {
            try
            {
                await DbContext.SaveChangesAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                throw new DataNotSavedException(ex);
            }
        }

        public void Transactional(Action<IUnitOfWork> action)
        {
            var transactionOptions = new TransactionOptions
            {
                IsolationLevel = IsolationLevel.ReadCommitted,
                // Timeout = new TimeSpan(0, 0, 0, 10),
            };

            using var scope = new TransactionScope(
                TransactionScopeOption.RequiresNew,
                transactionOptions,
                TransactionScopeAsyncFlowOption.Enabled);

            try
            {
                action.Invoke(this);
                scope.Complete();
            }
            catch (Exception ex)
            {
                scope.Dispose();
                throw new DataNotSavedException(ex);
            }
        }

    }
}
