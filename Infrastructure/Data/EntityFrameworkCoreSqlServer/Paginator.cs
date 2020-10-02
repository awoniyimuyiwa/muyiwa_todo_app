using Domain.Generic;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Infrastructure.Data.EntityFrameworkCoreSqlServer
{
    class Paginator<T>
    {
        public static PaginatedList<T> Paginate(
            IQueryable<T> source, int page = 1, int perPage = 15)
        {
            page = page >= 1 ? page : 1;
            perPage = (perPage >= 1 && perPage <= 15) ? perPage : 15;
            var count = source.LongCount();
            var items = source.Skip((page - 1) * perPage).Take(perPage).ToList();

            return new PaginatedList<T>(items, count, page, perPage);
        }

        public static async Task<PaginatedList<T>> PaginateAsync(
            IQueryable<T> source,
            int page = 1,
            int perPage = 15,
            CancellationToken cancellationToken = default)
        {
            page = page >= 1 ? page : 1;
            perPage = (perPage >= 1 && perPage <= 15) ? perPage : 15;
            var count = await source.LongCountAsync(cancellationToken);
            var items = await source.Skip((page - 1) * perPage).Take(perPage).ToListAsync(cancellationToken);

            return new PaginatedList<T>(items, count, page, perPage);
        }
    }
}
