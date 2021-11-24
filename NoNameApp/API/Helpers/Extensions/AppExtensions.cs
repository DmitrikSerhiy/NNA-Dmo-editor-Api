using System;
using Azure.Core;
using Azure.Extensions.AspNetCore.Configuration.Secrets;
using Azure.Security.KeyVault.Secrets;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace API.Helpers.Extensions {
    public static class AppExtensions {
        public static bool IsLocal(this IWebHostEnvironment env) {
            return env.EnvironmentName == "Local";
        }
        
        public static bool IsLocalMachine(this IWebHostEnvironment env) {
            return Environment.GetEnvironmentVariable("LocalMachine") == "true";
        }
        
        public static void AddNnaAzKeyVault(this IConfigurationBuilder configBuilder, TokenCredential creds) {
            var configuration = configBuilder.Build();
            var secretClient = new SecretClient(
                new Uri($"https://{configuration["az-key-vault"]}.vault.azure.net/"),
                creds);
            
            configBuilder.AddAzureKeyVault(secretClient, new KeyVaultSecretManager());
        }
    }
}