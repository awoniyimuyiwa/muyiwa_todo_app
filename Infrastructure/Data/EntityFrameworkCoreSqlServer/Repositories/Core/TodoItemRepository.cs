using Domain.Core;
using Domain.Core.Abstracts;
using Domain.Core.Dtos;
using Infrastructure.Utils.Abstracts;
using Microsoft.EntityFrameworkCore;
using Slugify;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Infrastructure.Data.EntityFrameworkCoreSqlServer.Repositories.Core
{
    class TodoItemRepository : Generic.BaseRepository<TodoItem, TodoItemDto>, ITodoItemRepository
    {
        readonly IRandomStringGenerator RandomStringGenerator;

        public TodoItemRepository(
            DbContext dbContext, IRandomStringGenerator randomStringGenerator) : base(dbContext) 
        {
            RandomStringGenerator = randomStringGenerator;
        }

        public override async Task<TodoItem> Add(
            TodoItem todoItem, CancellationToken cancellationToken = default)
        {
            todoItem = await base.Add(todoItem, cancellationToken);

            var slug = await GenerateSlug(todoItem.Name);
            DbContext.Entry(todoItem).Property(todoItem => todoItem.Slug).CurrentValue = slug;

            return todoItem;
        }

        /// If re-generating slugs on update is not a problem for SEO
        /** public override async void Update(TodoItem todoItem)
        {
            base.Update(todoItem);

            if (dbContext.Entry(todoItem).Property(todoItem => todoItem.Name).IsModified)
            {
                var slug = await GenerateSlug(todoItem.Name, todoItem.Id);
                dbContext.Entry(todoItem).Property(todoItem => todoItem.Slug).CurrentValue = slug;
            }
        }*/

        public void UpdateIsCompleted(TodoItem todoItem, bool status = true)
        {
            base.Update(todoItem);
            DbContext.Entry(todoItem).Property(todoItem => todoItem.IsCompleted).CurrentValue = status;
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
        protected override IQueryable<TodoItem> DoSearch(IQueryable<TodoItem> queryable, string search)
        {
            return queryable.Where(todoItem => EF.Functions.FreeText(todoItem.Name, search));
            // return queryable.Where(todoItem => EF.Functions.FreeText(todoItem.Name, search) || EF.Functions.FreeText(todoItem.AnotherField, search));
        }
        
        protected async Task<string> GenerateSlug(
            string name, 
            int? id = null, 
            CancellationToken cancellationToken = default)
        {
            // Though not compulsory, since multiple todo-items with the same name will probably be created but will have different execution day or time
            // In order to reduce the chance of collision with existing slugs, a random string is appended to the slug name.
            var randomString = RandomStringGenerator.Get(7);
            var slug = new SlugHelper().GenerateSlug($"{name} ${randomString}");

            // Ensure uniqueness
            List<string> similarSlugs = await DbSet
                .Where(todoItem => todoItem.Id != id)
                .Where(todoItem => EF.Functions.Like(todoItem.Slug, $"{slug}%"))
                .Select(todoItem => todoItem.Slug).AsNoTracking().ToListAsync(cancellationToken);

            if (!similarSlugs.Contains(slug)) { return slug; }

            var alternativeSlug = "";
            var suffix = 2;
            do
            {
                alternativeSlug = $"{slug}-{suffix}";
                suffix++;
            } while(similarSlugs.Contains(alternativeSlug));

            return alternativeSlug;
        }
    }
}
