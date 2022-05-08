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
using API.Helpers;
using API.Helpers.Extensions;
using Infrastructure;
using SendGrid.Extensions.DependencyInjection;

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
            services.Configure<SendGridConfiguration>(_configuration.GetSection(nameof(SendGridConfiguration)));

            services.AddNnaAuthenticationOptions();
            if (_environment.IsLocal()) {
                services.AddNnaLocalLoggerOptions(_environment, _configuration);
            } 
            
            services
                .AddSignalR()
                .AddMessagePackProtocol()
                .AddHubOptions<EditorHub>(o => {
                    o.EnableDetailedErrors = true;
                });
            services.AddAutoMapper(typeof(Startup));
            services.AddSendGrid(options => {
                options.ApiKey = _configuration.GetValue<string>("SendGridConfiguration:ApiKey");
            });
            services.AddNnaMvcAndFilters();

            services.AddHostedService<LifetimeEventsManager>();

            builder.Populate(services);
            builder.RegisterModule(new GlobalModule());
            builder.RegisterModule(new ApiModule());
            return new AutofacServiceProvider(builder.Build());
        }

        public void Configure(IApplicationBuilder app) {
            app.UseDeveloperExceptionPage(); // todo: use only for non-prod env. when global errors handling strategy is ready.
            app.UseHttpsRedirection();
            app.UseHsts();

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
