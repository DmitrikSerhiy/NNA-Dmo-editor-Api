using API.Helpers.Extensions;
using Autofac.Extensions.DependencyInjection;
using Azure.Identity;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace API {
    public class Program {
        public static void Main(string[] args) {
            Host.CreateDefaultBuilder(args)
                .UseServiceProviderFactory(new AutofacServiceProviderFactory())
                .ConfigureWebHostDefaults(webHostBuilder => {
                    webHostBuilder.UseSerilog();
                    webHostBuilder.ConfigureAppConfiguration((context, config) => {
                        if (context.HostingEnvironment.IsLocalMachine() && !context.HostingEnvironment.IsLocal()) {
                            // for local run with develop env
                            config.AddNnaAzKeyVault(new AzureCliCredential());
                        }
                        else if (!context.HostingEnvironment.IsLocalMachine()) {
                            // for develop run with develop env
                            config.AddNnaAzKeyVault(new DefaultAzureCredential());
                        }

                        if (context.HostingEnvironment.IsLocal()) {
                            config.AddUserSecrets("9287554d-fee5-4c75-8bd9-0ecbd051c423");
                        }
                    });
                    webHostBuilder.UseStartup<Startup>();
                })
                .Build()
                .Run();
        }
    }
}


