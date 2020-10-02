using Domain.Generic.Auth.Dtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Generic.Auth
{
    public class User : BaseEntity<UserDto>
    {
        public string IdFromIdp { get; set; }

        public override UserDto ToDto()
        {
            return new UserDto()
            {
                Slug = IdFromIdp,
                CreatedAt = CreatedAt != null ? Formatter.Format(CreatedAt) : null,
                UpdatedAt = UpdatedAt != null ? Formatter.Format(UpdatedAt) : null
            };
        }
    }
}
