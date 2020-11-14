using API.Infrastructure;
using API.Infrastructure.Authentication;
using API.Infrastructure.Extensions;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using API.Hubs;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.AspNetCore.SignalR.Protocol;

namespace API
{
    public class Startup {

        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _environment;
        private readonly string angularClientOrigin = "angularClient";
        public Startup(
            IWebHostEnvironment environment) {
            _environment = environment ?? throw new ArgumentNullException(nameof(environment));
            var builder = new ConfigurationBuilder()
                .SetBasePath(_environment.ContentRootPath)
                .AddJsonFile("appsettings.json", true, true)
                .AddJsonFile($"appsettings.{_environment.EnvironmentName}.json", true)
                .AddEnvironmentVariables();
            _configuration = builder.Build();
        }

        public IServiceProvider ConfigureServices(IServiceCollection services) {
            var builder = new ContainerBuilder();
            services.AddLoggerOptions(_environment, _configuration);
            services.AddDbOptions(_configuration);
            services.AddCors(o => {
                o.AddPolicy(angularClientOrigin, policyBuilder => {
                    policyBuilder.WithOrigins("http://localhost:4200", "http://nna-front-bucket1.s3-website.eu-central-1.amazonaws.com");
                    policyBuilder.AllowAnyMethod();
                    policyBuilder.AllowAnyHeader();
                    policyBuilder.AllowCredentials();
                });
            });
            services.AddAuthenticationOptions();
            services.AddSignalR().AddHubOptions<EditorHub>(o => {
                o.EnableDetailedErrors = true;
                o.SupportedProtocols = new List<string>{ new JsonHubProtocol().Name };
            });
            services.AddAutoMapper(typeof(Startup));
            services.AddMvcAndFilters();

            builder.Populate(services);
            builder.RegisterModule(new AutofacModule());
            var applicationContainer = builder.Build();
            return new AutofacServiceProvider(applicationContainer);
        }

        public void Configure(IApplicationBuilder app) {
            app.UseDeveloperExceptionPage();
            //app.UseHttpsRedirection();
            //app.UseHsts();

            app.UseRouting();
            app.UseCors(angularClientOrigin);

            app.UseAuthentication();
            app.UseAuthorization();
            app.UseMiddleware<AuthenticatedIdentityMiddleware>();

            app.UseEndpoints(endpoints => {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller}/{action=Index}/{id?}");
                endpoints.MapHub<EditorHub>("api/editor", o => {
                    o.Transports = HttpTransportType.WebSockets | HttpTransportType.LongPolling | HttpTransportType.ServerSentEvents;
                    //todo: consider use Redis backplane when api is scale out (2 or more EC2)
                });
            });
        }
    }
}
