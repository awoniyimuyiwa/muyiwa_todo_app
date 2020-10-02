using Domain.Core;
using Domain.Generic.Auth;
using Infrastructure.Data.EntityFrameworkCoreSqlServer.Mappings.Core;
using Infrastructure.Data.EntityFrameworkCoreSqlServer.Mappings.Generic.Auth;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data.EntityFrameworkCoreSqlServer
{
    class DbContext : Microsoft.EntityFrameworkCore.DbContext
    {
        public DbSet<TodoItem> TodoItems { get; set; }
        public DbSet<User> Users { get; set; }

        public DbContext(DbContextOptions<DbContext> options) : base(options)
        {
            // Increase timeout, seems full text search with EF.Functions.FreeText() is timing out on first search query
            Database.SetCommandTimeout(9000);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            new TodoItemMapping(modelBuilder.Entity<TodoItem>());
            new UserMapping(modelBuilder.Entity<User>());
        }
    }
}
