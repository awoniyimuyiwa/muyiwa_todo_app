using Domain.Generic.Auth.Dtos;

namespace Domain.Generic.Auth.Abstracts
{
    public interface IUserRepository : IRepository<User, UserDto>
    {
        
    }
}
