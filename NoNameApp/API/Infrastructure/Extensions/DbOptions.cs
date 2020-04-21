using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Persistence;

namespace API.Infrastructure.Extensions {
    public static class DbOptions {
        public static void AddDbOptions(this IServiceCollection services, string connectionString) {
            services.AddDbContext<NoNameContext>(options => {
                options.UseMySql(connectionString, mySqlOptions => {
                    mySqlOptions.EnableRetryOnFailure(1);
                    mySqlOptions.MigrationsAssembly("Persistence");
                });
            });
        }
    }
}
