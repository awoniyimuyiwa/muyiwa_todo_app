using Application.Services.Abstracts;
using Domain.Core;
using Domain.Core.Abstracts;
using Domain.Core.Dtos;
using Domain.Generic.Auth;
using Domain.Generic.Auth.Abstracts;
using Infrastructure.Data.Abstracts;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Services
{
    class TodoItemService : ITodoItemService
    {
        public TodoItemService(IUnitOfWork uow)
        {
            Uow = uow;
        }

        readonly IUnitOfWork Uow;
        public ITodoItemRepository TodoItemRepository => Uow.TodoItemRepository;
        public IUserRepository UserRepository => Uow.UserRepository;

        public async Task<TodoItem> Create(
            User user,
            TodoItemDto todoItemDto, 
            CancellationToken cancellationToken = default)
        {
            var todoItem = Map(todoItemDto);
            todoItem.User = user;

            todoItem = await TodoItemRepository.Add(todoItem, cancellationToken);
            await Uow.Commit(cancellationToken);

            return todoItem;
        }

        public Task Update(TodoItem todoItem, TodoItemDto todoItemDto, CancellationToken cancellationToken = default)
        {
            Map(todoItemDto, todoItem);

            TodoItemRepository.Update(todoItem);

            return Uow.Commit(cancellationToken);
        }

        public Task UpdateIsCompleted(
            TodoItem todoItem, 
            bool status = true, 
            CancellationToken cancellationToken = default)
        {
            TodoItemRepository.UpdateIsCompleted(todoItem, status);

            return Uow.Commit(cancellationToken);
        }

        public Task Delete(TodoItem todoItem, CancellationToken cancellationToken = default)
        {
            TodoItemRepository.Delete(todoItem);

            return Uow.Commit(cancellationToken);
        }

        private TodoItem Map(TodoItemDto todoItemDto, TodoItem todoItem = null)
        {
            todoItem ??= new TodoItem();
            todoItem.Name = todoItemDto.Name;
            // Fetch and set other relationship properties here too

            return todoItem;
        }
    }
}
