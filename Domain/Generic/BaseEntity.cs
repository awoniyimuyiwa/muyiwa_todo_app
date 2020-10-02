using System;

namespace Domain.Generic
{
    public abstract class BaseEntity<R> where R : BaseDto
    {
        public int Id { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime UpdatedAt { get; private set; }
        public abstract R ToDto();
    }
}
