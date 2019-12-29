using Autofac;
using Autofac.Features.ResolveAnything;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Model;
using Persistence;

namespace API.Infrastructure {
    public class AutofacModule : Module {
        public AutofacModule() { }
        protected override void Load(ContainerBuilder builder) {

            builder
                .RegisterType<LoggerFactory>()
                .As<ILoggerFactory>()
                .UsingConstructor()
                .InstancePerLifetimeScope();

            builder
                .RegisterType<UnitOfWork>()
                .InstancePerLifetimeScope();

            //builder
            //    .RegisterType<IUserStore>()
            //    .Ins

            builder
                .RegisterType<UserManager<ApplicationUser>>()
                .AsSelf()
                .InstancePerLifetimeScope();

           // builder.RegisterControllers(typeof(MvcApplication).Assembly);

            builder.RegisterSource(
                new AnyConcreteTypeNotAlreadyRegisteredSource()
                    .WithRegistrationsAs(b => b.InstancePerLifetimeScope()));
        }
    }
}
