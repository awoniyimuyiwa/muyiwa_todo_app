using Application.Services.Abstracts;
using Domain.Core;
using Domain.Generic.Auth;
using Infrastructure.Data.Abstracts;
using Infrastructure.Utils.Abstracts;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.Services
{
    public class RepositorySeeder : IRepositorySeeder
    {
        readonly IUnitOfWork Uow;
        readonly IRandomStringGenerator RandomStringGenerator;

        public RepositorySeeder(
            IUnitOfWork uow,
            IRandomStringGenerator randomStringGenerator)
        {
            Uow = uow;
            RandomStringGenerator = randomStringGenerator;
        }

        public async Task Seed()
        {
            await SeedTodoItems();
        }

        private async Task SeedTodoItems()
        {
            if (!await Uow.UserRepository.IsEmpty()) { return; }

            var randomUser = await Uow.UserRepository.Add(new User
            {
                IdFromIdp = $"random-{RandomStringGenerator.Get(14)}"
            });

            var todoItems = new TodoItem[]
            {
                new TodoItem{ Name="Sleep", User = randomUser },
                new TodoItem{ Name="Eat", User = randomUser },
                new TodoItem{ Name="Drink", User = randomUser }
            };

            List<Task<TodoItem>> tasks = new List<Task<TodoItem>>();
            foreach (TodoItem todoItem in todoItems)
            {
                tasks.Add(Uow.TodoItemRepository.Add(todoItem));
            }

            await Task.WhenAll(tasks);
            await Uow.Commit();
        }
    }
}
