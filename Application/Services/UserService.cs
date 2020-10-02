using Application.Services.Abstracts;
using Domain.Generic.Auth;
using Domain.Generic.Auth.Abstracts;
using Domain.Generic.Auth.Dtos;
using Infrastructure.Data.Abstracts;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Services
{
    class UserService : IUserService
    {
        public UserService(IUnitOfWork uow)
        {
            Uow = uow;
        }

        readonly IUnitOfWork Uow;
        public IUserRepository UserRepository => Uow.UserRepository;

        public async Task<User> FindOrCreate(
           string idFromIdp, 
           UserDto userDto,
           CancellationToken cancellationToken = default)
        {
            var user = await UserRepository.FindOneBy(u => u.IdFromIdp == idFromIdp);
            if (user == null)
            {
                user = Map(userDto);
                user.IdFromIdp = idFromIdp;
                user = await UserRepository.Add(user, cancellationToken);
                await Uow.Commit(cancellationToken);
            }

            return user;
        }

        public Task Update(User user, UserDto userDto, CancellationToken cancellationToken = default)
        {
            Map(userDto, user);

            UserRepository.Update(user);

            return Uow.Commit(cancellationToken);
        }

        public Task Delete(User user, CancellationToken cancellationToken = default)
        {
            UserRepository.Delete(user);

            return Uow.Commit(cancellationToken);
        }

        private User Map(UserDto userDto, User user = null)
        {
            user ??= new User();
            // Fetch and set other relationship properties here too

            return user;
        }
    }
}
