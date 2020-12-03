using Autofac;
using Autofac.Features.ResolveAnything;

namespace Model {
    public class ModelModule : Module {

        protected override void Load(ContainerBuilder builder) {

            builder.RegisterSource(
                new AnyConcreteTypeNotAlreadyRegisteredSource()
                    .WithRegistrationsAs(b => b.InstancePerLifetimeScope()));
        }
    }
}
