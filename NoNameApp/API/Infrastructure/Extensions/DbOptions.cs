using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Persistence;

namespace API.Infrastructure.Extensions {
    public static class DbOptions {
        public static void AddDbOptions(this IServiceCollection services, IConfiguration configuration) {
            services.AddDbContext<NnaContext>(options => {
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"), sqlOptions => {
                    sqlOptions.EnableRetryOnFailure(1);
                    sqlOptions.MigrationsAssembly("Persistence");
                });
            });
        }
    }
}
