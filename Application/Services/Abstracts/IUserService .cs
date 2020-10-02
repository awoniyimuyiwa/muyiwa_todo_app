using Domain.Generic.Auth;
using Domain.Generic.Auth.Abstracts;
using Domain.Generic.Auth.Dtos;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Services.Abstracts
{
    public interface IUserService
    {
        /// <summary>
        /// 
        /// </summary>
        IUserRepository UserRepository { get; }

        /// <summary>
        /// Returns a user with the idFromIdp if it already exists. If it doesn't exist, it creates it first.
        /// </summary>
        /// <param name="idFromIdp"></param>
        /// <param name="userDto"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<User> FindOrCreate(
            string idFromIdp, 
            UserDto userDto,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="user"></param>
        /// <param name="userDto"></param>
        /// <param name="cancellationToken"></param>
        Task Update(
            User user, 
            UserDto userDto, 
            CancellationToken cancellationToken = default);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="user"></param>
        /// <param name="cancellationToken"></param>
        Task Delete(User user, CancellationToken cancellationToken = default);
    }
}
