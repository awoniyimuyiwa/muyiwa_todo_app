using Infrastructure.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace Application
{
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, string connectionString)
        {
            services.AddInfrastructureServices(connectionString);

            // One instance of a service class per request
            services.AddScoped<Services.Abstracts.IUserService, Services.UserService>();
            services.AddScoped<Services.Abstracts.ITodoItemService, Services.TodoItemService>();
            services.AddScoped<Services.Abstracts.IRepositorySeeder, Services.RepositorySeeder>();

            return services;
        }
    }
}
