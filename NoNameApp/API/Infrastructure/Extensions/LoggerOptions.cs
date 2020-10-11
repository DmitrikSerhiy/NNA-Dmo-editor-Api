using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace API.Infrastructure.Extensions
{
    public static class LoggerOptions {
        public static void AddLoggerOptions(this IServiceCollection services, IWebHostEnvironment environment, IConfiguration configuration) {
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(configuration)
                .CreateLogger();
        }
    }
}
