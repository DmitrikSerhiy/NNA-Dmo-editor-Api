using System;
using API.Helpers.Extensions;
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
                    if (!context.HostingEnvironment.IsLocal()) {
                        var builtConfig = config.Build();
                        var secretClient = new SecretClient(
                            new Uri($"https://{builtConfig["az-key-vault"]}.vault.azure.net/"),
                            new DefaultAzureCredential());
                            // new AzureCliCredential()); // for local debugging with dev db
                        config.AddAzureKeyVault(secretClient, new KeyVaultSecretManager());
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


