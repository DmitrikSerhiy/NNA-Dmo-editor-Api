using System;
using API.Helpers;
using API.Infrastructure;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SpaServices.AngularCli;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Persistence;

namespace API {
    public class Startup {

        public IConfiguration Configuration { get; }
        public Startup(IConfiguration configuration) {
            Configuration = configuration;
        }

        public IServiceProvider ConfigureServices(IServiceCollection services) {
            var connectionString = Configuration.GetConnectionString("DefaultConnection");
            services.AddDbContext<NoNameContext>(options => {
                options.UseMySql(connectionString, mySqlOptions => {
                    mySqlOptions.EnableRetryOnFailure(1);
                    mySqlOptions.MigrationsAssembly("Persistence");
                });
            });

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options => {
                    options.RequireHttpsMetadata = false;//todo: swap to true
                    options.TokenValidationParameters = new TokenValidationParameters {
                        ValidateIssuer = true,
                        ValidIssuer = AuthOptions.ISSUER,
                        ValidateAudience = true,
                        ValidAudience = AuthOptions.AUDIENCE,
                        ValidateLifetime = true,
                        IssuerSigningKey = AuthOptions.GetSymmetricSecurityKey(),
                        ValidateIssuerSigningKey = true,
                    };
                });

            services.Configure<IdentityOptions>(options => {
                // todo: add some password restrictions later
                options.User.RequireUniqueEmail = true;
            });

            services.AddControllersWithViews();
            // In production, the Angular files will be served from this directory
            services.AddSpaStaticFiles(configuration => {
                configuration.RootPath = "ClientApp/dist";
            });

            var builder = new ContainerBuilder();
            builder.Populate(services);
            builder.RegisterModule(new AutofacModule());
            var applicationContainer = builder.Build();

            return new AutofacServiceProvider(applicationContainer);
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env) {
            if (env.IsDevelopment()) {
                app.UseDeveloperExceptionPage();
            } else {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            if (!env.IsDevelopment()) {
                app.UseSpaStaticFiles();
            }

            app.UseRouting();
            app.UseAuthentication();


            app.UseEndpoints(endpoints => {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller}/{action=Index}/{id?}");
            });

            app.UseSpa(spa => {
                // To learn more about options for serving an Angular SPA from ASP.NET Core,
                // see https://go.microsoft.com/fwlink/?linkid=864501

                spa.Options.SourcePath = "ClientApp";

                if (env.IsDevelopment()) {
                    spa.UseAngularCliServer(npmScript: "start");
                }
            });
        }
    }
}
