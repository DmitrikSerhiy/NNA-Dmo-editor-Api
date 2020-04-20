using API.Infrastructure;
using API.Infrastructure.Authentication;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using AutoMapper;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Model.Entities;
using Persistence;
using System;
using Microsoft.Extensions.Logging;

namespace API
{
    public class Startup {

        private readonly IConfiguration _configuration;
        private readonly string angularClientOrigin = "angularClient";
        public Startup(IHostingEnvironment env) {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", true, true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", true)
                .AddEnvironmentVariables();
            _configuration = builder.Build();
        }

        public IServiceProvider ConfigureServices(IServiceCollection services) {
            //var connectionString =
            //    "Server=nna-dev.c2lvxmxgqxkc.eu-central-1.rds.amazonaws.com;Port=3306;Database=nnaDevDB;Uid=superadmin;Pwd=1q2w3eazsxdc";

            var connectionString = _configuration.GetConnectionString("DefaultConnection");
            services.AddDbContext<NoNameContext>(options => {
                options.UseMySql(connectionString, mySqlOptions => {
                    mySqlOptions.EnableRetryOnFailure(1);
                    mySqlOptions.MigrationsAssembly("Persistence");
                });
            });

            var identityBuilder = services
                .AddIdentity<NoNameUser, NoNameRole>(options => {
                    // todo: add some password restrictions later
                    options.User.RequireUniqueEmail = true;
                    options.Password.RequireDigit = false;
                    options.Password.RequireLowercase = false;
                    options.Password.RequiredLength = 3;
                    options.Password.RequireNonAlphanumeric = false;
                    options.Password.RequireUppercase = false;
                })
                .AddEntityFrameworkStores<NoNameContext>();
            //todo: add token provider for future Maybe I should use IdentityServer4 here
            identityBuilder.AddUserManager<NoNameUserManager>();

            services.AddAuthentication(options => {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(options => {
                    options.RequireHttpsMetadata = false;//todo: swap to true
                    options.TokenValidationParameters = new TokenValidationParameters {
                        ValidateIssuer = true,
                        ValidIssuer = AuthOptions.ISSUER,
                        ValidateAudience = true,
                        ValidAudience = AuthOptions.AUDIENCE,
                        ValidateLifetime = false,
                        //todo: change it later
                        IssuerSigningKey = AuthOptions.GetSymmetricSecurityKey(),
                        ValidateIssuerSigningKey = true,
                    };
                });

            services.AddCors(o => {
                o.AddPolicy(angularClientOrigin, policyBuilder => {
                    policyBuilder.WithOrigins("http://localhost:4200");
                    policyBuilder.AllowAnyMethod();
                    policyBuilder.AllowAnyHeader();
                });
            });

            services.AddAutoMapper(typeof(Startup));

            services
                .AddControllersWithViews(options => {
                    options.Filters.Add(typeof(ExceptionFilter));
                    options.Filters.Add(typeof(TransactionFilter));
                })
                .AddFluentValidation(fv => fv.RegisterValidatorsFromAssemblyContaining<Startup>());

            var builder = new ContainerBuilder();
            builder.Populate(services);
            builder.RegisterModule(new AutofacModule());
            var applicationContainer = builder.Build();

            return new AutofacServiceProvider(applicationContainer);
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env) {
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
