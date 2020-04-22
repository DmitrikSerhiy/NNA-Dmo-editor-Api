using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Serilog;

namespace API
{
    public class Program {
        public static void Main(string[] args) {
            WebHost.CreateDefaultBuilder(args)
                .UseSerilog()
                .UseStartup<Startup>()
                .ConfigureServices(services => services.AddAutofac())
                .Build()
                .Run();
        }
    }
}


