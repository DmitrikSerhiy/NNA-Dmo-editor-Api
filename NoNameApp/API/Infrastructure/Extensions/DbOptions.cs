using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Persistence;

namespace API.Infrastructure.Extensions {
    public static class DbOptions {
        public static void AddDbOptions(this IServiceCollection services, IConfiguration configuration) {
            services.AddDbContext<NoNameContext>(options => {
                options.UseMySql(configuration.GetConnectionString("DefaultConnection"), mySqlOptions => {
                    mySqlOptions.EnableRetryOnFailure(1);
                    mySqlOptions.MigrationsAssembly("Persistence");
                });
            });
        }
    }
}
