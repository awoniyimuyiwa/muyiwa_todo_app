using Domain.Core;
using Domain.Core.Abstracts;
using Domain.Core.Dtos;
using Domain.Generic.Auth;
using Domain.Generic.Auth.Abstracts;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Services.Abstracts
{
    public interface ITodoItemService
    {
        /// <summary>
        /// 
        /// </summary>
        ITodoItemRepository TodoItemRepository { get; }

        /// <summary>
        /// 
        /// </summary>
        IUserRepository UserRepository { get; }

        /// <summary>
        /// Create todo-item
        /// </summary>
        /// <param name="user"></param>
        /// <param name="todoItemDto"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<TodoItem> Create(
            User user,
            TodoItemDto todoItemDto, 
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Update todo-item
        /// </summary>
        /// <param name="todoItem"></param>
        /// <param name="todoItemDto"></param>
        /// <param name="cancellationToken"></param>
        Task Update(
            TodoItem todoItem, 
            TodoItemDto todoItemDto, 
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Set the completed status of a todo-item
        /// </summary>
        /// <param name="todoItem"></param>
        /// <param name="status"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task UpdateIsCompleted(
            TodoItem todoItem, 
            bool status = true, 
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Delete todo-item
        /// </summary>
        /// <param name="todoItem"></param>
        /// <param name="cancellationToken"></param>
        Task Delete(TodoItem todoItem, CancellationToken cancellationToken = default);
    }
}
