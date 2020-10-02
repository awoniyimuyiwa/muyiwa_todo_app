using Domain.Generic.Auth;
using Domain.Generic.Auth.Abstracts;
using Domain.Generic.Auth.Dtos;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Infrastructure.Data.EntityFrameworkCoreSqlServer.Repositories.Core
{
    class UserRepository : Generic.BaseRepository<User, UserDto>, IUserRepository
    {
        public UserRepository(DbContext dbContext) : base(dbContext) {}

        public override async Task<User> Add(
            User user, CancellationToken cancellationToken = default)
        {
            user = await base.Add(user, cancellationToken);

            return user;
        }

        /// <summary>
        /// Note that EF.Functions.FreeText relies on full text indexing feature in MSSQLServer
        /// which improves search performance compared to using sql like to search. 
        /// 
        /// To start using full text search, full text indexing has to be enabled in SQL server (available in Developer edition and above, not availabale in SQLExpress),
        /// if full text indexing is not already enabled you will need to run the sql server installation centre again, click on installation->New sql server stand alone installation or add features to an existing installation.
        /// When asked for installation directory mine was C:\SQL2019\Developer_ENU.
        /// Step through the wizard until feature selection and tick the check-box for full-text indexing. 
        /// After that you also need to create a full text catlogue for your DB, you can name it TodoAppFullTextCatalogue. See more here:
        /// https://docs.microsoft.com/en-us/sql/relational-databases/search/get-started-with-full-text-search?view=sql-server-ver15
        /// </summary>
        /// <param name="queryable"></param>
        /// <param name="search"></param>
        /// <returns></returns>
        protected override IQueryable<User> DoSearch(IQueryable<User> queryable, string search)
        {
            return queryable.Where(user => EF.Functions.FreeText(user.IdFromIdp, search));           
        }
    }
}
