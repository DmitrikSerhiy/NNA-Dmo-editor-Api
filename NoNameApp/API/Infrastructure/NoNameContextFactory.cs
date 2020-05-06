using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Persistence;

namespace API.Infrastructure {
    // ReSharper disable once UnusedMember.Global
    // this is for manual migrations
    public class NoNameContextFactory : IDesignTimeDbContextFactory<NoNameContext> {
        public NoNameContext CreateDbContext(string[] args) {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "../API"))
                .AddJsonFile("appsettings.Development.json");
            var configuration = builder.Build();

            var optionsBuilder = new DbContextOptionsBuilder<NoNameContext>();
            optionsBuilder.UseMySql(configuration.GetConnectionString("DefaultConnection"),
                mySqlOptions => {
                    mySqlOptions.EnableRetryOnFailure(1);
                    mySqlOptions.MigrationsAssembly("Persistence");
                });
            return new NoNameContext(optionsBuilder.Options);
        }
    }
}
