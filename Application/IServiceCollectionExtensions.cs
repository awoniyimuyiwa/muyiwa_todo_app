using Microsoft.Extensions.DependencyInjection;

namespace Application
{
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            // One instance of a service class per request
            services.AddScoped<Services.Abstracts.IUserService, Services.UserService>();
            services.AddScoped<Services.Abstracts.ITodoItemService, Services.TodoItemService>();
            services.AddScoped<Services.Abstracts.IRepositorySeeder, Services.RepositorySeeder>();

            return services;
        }
    }
}
