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
using Serilog;
using System;
using System.Linq;

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
            Log.Information("Some random shit once again");
            var connectionString =
                "Server=nna-dev.c2lvxmxgqxkc.eu-central-1.rds.amazonaws.com;Port=3306;Database=nnaDevDB;Uid=superadmin;Pwd=1q2w3eazsxdc";
            
            var cs3 = _configuration.GetSection("ConnectionStrings")["DefaultConnection"];

            //var cs1 = _configuration.GetConnectionString("DefaultConnection");
            //var cs2 = _configuration.GetChildren().FirstOrDefault(c => c["ConnectionStrings"] == "DefaultConnection");

            Log.Warning(cs3);
            //Log.Warning(cs2);

            services.AddDbOptions(connectionString);
            services.AddAuthenticationOptions();
            services.AddCors(o => {
                o.AddPolicy(angularClientOrigin, policyBuilder => {
                    policyBuilder.WithOrigins("http://localhost:4200");
                    policyBuilder.AllowAnyMethod();
                    policyBuilder.AllowAnyHeader();
                });
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
            });
        }
    }
}
