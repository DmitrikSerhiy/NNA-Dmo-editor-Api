using Autofac;
using Autofac.Features.ResolveAnything;
using Microsoft.Extensions.Logging;
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

            builder.RegisterSource(
                new AnyConcreteTypeNotAlreadyRegisteredSource()
                    .WithRegistrationsAs(b => b.InstancePerLifetimeScope()));
        }
    }
}
