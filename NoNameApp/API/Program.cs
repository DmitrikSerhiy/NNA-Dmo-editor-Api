using System;
using Autofac.Extensions.DependencyInjection;
using Azure.Extensions.AspNetCore.Configuration.Secrets;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace API
{
    public class Program {
        public static void Main(string[] args) {
            WebHost.CreateDefaultBuilder(args)
                .UseSerilog()
                .UseStartup<Startup>()
                .ConfigureAppConfiguration((context, config) =>
                {
                    if (context.HostingEnvironment.EnvironmentName.Equals("Development")) {
                        var builtConfig = config.Build();
                        var secretClient = new SecretClient(
                            new Uri($"https://{builtConfig["az-key-vault"]}.vault.azure.net/"),
                            new DefaultAzureCredential());
                            // new AzureCliCredential());
                        config.AddAzureKeyVault(secretClient, new KeyVaultSecretManager());
                    }
                })                
                .ConfigureServices(services => services.AddAutofac())
                .Build()
                .Run();
        }
    }
}


