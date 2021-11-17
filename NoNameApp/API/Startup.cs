using API.Features.Editor.Hubs;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using API.Features.Account.Services;
using API.Features.Account.Services.Local;
using API.Helpers;
using API.Helpers.Extensions;
using Infrastructure;

namespace API {
    public class Startup {

        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _environment;

        public Startup(
            IWebHostEnvironment environment,
            IConfiguration configuration) {
            _environment = environment ?? throw new ArgumentNullException(nameof(environment));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        public IServiceProvider ConfigureServices(IServiceCollection services) {
            var builder = new ContainerBuilder();
            services.AddNnaDbOptions(_configuration);
            services.AddNnaCorsOptions(_configuration);
            services.Configure<JwtOptions>(_configuration.GetSection(nameof(JwtOptions)));
            
            if (_environment.IsLocal()) {
                services.AddNnaLocalLoggerOptions(_environment, _configuration);
                services.AddNnaLocalAuthenticationOptions(_configuration);
            } else {
                services.AddNnaAuthenticationOptions();
            }
            
            services
                .AddSignalR()
                .AddHubOptions<EditorHub>(o => {
                    o.EnableDetailedErrors = true;
                });
            services.AddAutoMapper(typeof(Startup));
            services.AddNnaMvcAndFilters();

            builder.Populate(services);
            builder.RegisterModule(new GlobalModule());
            builder.RegisterModule(new ApiModule());
            return new AutofacServiceProvider(builder.Build());
        }

        public void Configure(IApplicationBuilder app) {
            app.UseDeveloperExceptionPage();
            app.UseHttpsRedirection();
            app.UseHsts();

            if (_environment.IsLocal()) {
                app.UseNnaAccountRewriteOptions();
            }
            
            app.UseRouting();
            app.UseNnaCorsOptions();
            
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseMiddleware<AuthenticatedIdentityMiddleware>();

            app.UseEndpoints(endpoints => {
                endpoints.MapControllers();
                endpoints.MapHub<EditorHub>("api/editor", options => {
                    options.Transports = HttpTransportType.WebSockets;
                });
            });
        }
    }
}
