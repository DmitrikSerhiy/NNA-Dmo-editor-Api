using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.SpaServices.Webpack;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace API {
    public class Startup {
        public void ConfigureServices(IServiceCollection services) {
            var connectionString = "server=127.0.0.1;user id=root;password=98986476Serega;port=3306;database=noNameApp;";
            services.AddDbContext<NoNameContext>(options => options.UseMySql(connectionString));

            services.AddMvc(o => o.EnableEndpointRouting = false);
        }

        [System.Obsolete]
        public void Configure(IApplicationBuilder app, IHostingEnvironment env) {
            if (env.IsDevelopment()) {
                app.UseDeveloperExceptionPage();

                app.UseWebpackDevMiddleware(new WebpackDevMiddlewareOptions {
                    HotModuleReplacement = true
                });
            }

            app.UseDefaultFiles();
            app.UseStaticFiles();
            app.UseMvc();
        }
    }
}