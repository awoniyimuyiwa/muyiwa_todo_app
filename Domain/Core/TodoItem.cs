using Domain.Core.Dtos;
using Domain.Generic;
using Domain.Generic.Auth;

namespace Domain.Core
{
    public class TodoItem : BaseEntity<TodoItemDto>
    {
        private string name;
        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                name = value;
                NormalizedName = value.ToUpperInvariant();
            }
        }
        // Unique constraint will be on this property rather than on Name property so as to ensure case-insensitivity of Name Property
        public string NormalizedName { get; private set; }

        public bool IsCompleted { get; private set; }
        public string Slug { get; private set; } 
        public virtual User User { get; set; }

        public override TodoItemDto ToDto()
        {
            return new TodoItemDto()
            {
                Name = Name,
                IsCompleted = IsCompleted,
                Slug = Slug,
                CreatedAt = CreatedAt != null ? Formatter.Format(CreatedAt) : null,
                UpdatedAt = UpdatedAt != null ? Formatter.Format(UpdatedAt) : null
            };
        }
    }
}
