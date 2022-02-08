using API.Helpers.Extensions;
using Autofac.Extensions.DependencyInjection;
using Azure.Identity;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Serilog;

namespace API {
    public class Program {
        public static void Main(string[] args) {
            WebHost.CreateDefaultBuilder(args)
                .UseSerilog()
                .UseStartup<Startup>()
                .ConfigureAppConfiguration((context, config) =>
                {
                    if (context.HostingEnvironment.IsLocalMachine() && !context.HostingEnvironment.IsLocal()) {
                        // for local run with develop env
                        config.AddNnaAzKeyVault(new AzureCliCredential()); 
                    } else if (!context.HostingEnvironment.IsLocalMachine()) {
                        // for develop run with develop env
                        config.AddNnaAzKeyVault(new DefaultAzureCredential()); 
                    }
                    
                    if (context.HostingEnvironment.IsLocal()) {
                        config.AddUserSecrets("9287554d-fee5-4c75-8bd9-0ecbd051c423");
                    }
                })                
                .ConfigureServices(services => services.AddAutofac())
                .Build()
                .Run();
        }
    }
}


