using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Persistence;

namespace API.Infrastructure {
    // ReSharper disable once UnusedMember.Global
    // this is for manual migrations
    public class MigrationHelper : IDesignTimeDbContextFactory<NnaContext> {
        public NnaContext CreateDbContext(string[] args) {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "../API"))
                .AddJsonFile("appsettings.Development.json");
            var configuration = builder.Build();

            var optionsBuilder = new DbContextOptionsBuilder<NnaContext>();
            optionsBuilder.UseSqlServer(configuration.GetConnectionString("DefaultConnection"),
                mySqlOptions => {
                    mySqlOptions.EnableRetryOnFailure(1);
                    mySqlOptions.MigrationsAssembly("Persistence");
                });
            return new NnaContext(optionsBuilder.Options);
        }
    }
}
