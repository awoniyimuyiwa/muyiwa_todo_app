using Domain.Core.Dtos;
using Domain.Generic;

namespace Domain.Core.Abstracts
{
    public interface ITodoItemRepository : IRepository<TodoItem, TodoItemDto>
    {
        public void UpdateIsCompleted(TodoItem todoItem, bool status = true);
    }
}
