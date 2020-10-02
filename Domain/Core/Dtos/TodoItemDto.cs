
using Domain.Generic;

namespace Domain.Core.Dtos
{
    public class TodoItemDto : BaseDto
    {
        public string Name { get; set; }
        public bool IsCompleted { get; set; }
    }
}
