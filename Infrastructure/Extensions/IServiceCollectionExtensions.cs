using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Extensions
{
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, string connectionString)
        {
            // AddBbContext adds dbConetxt as scoped i.e one per request
            services.AddDbContext<Data.EntityFrameworkCoreSqlServer.DbContext>(options => options.UseSqlServer(connectionString).UseLazyLoadingProxies());
            
            // One instance of String generator per request (real random generators invoke heavy APIs BCryptGenRandom on Windows, OpenSSL on other platforms. That's why using interfaces which gives ability to mock them during test is important.
            services.AddScoped<Utils.Abstracts.IRandomStringGenerator, Utils.RandomStringGenerator>();

            // One instance of UnitOfWork per request
            services.AddScoped<Data.Abstracts.IUnitOfWork, Data.EntityFrameworkCoreSqlServer.UnitOfWork>();

            return services;
        }
    }
}
