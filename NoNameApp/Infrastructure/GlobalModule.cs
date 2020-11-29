using Autofac;
using Model;
using Persistence;

namespace Infrastructure
{
    public class GlobalModule : Module
    {
        protected override void Load(ContainerBuilder builder) {

            builder.RegisterModule(new ModelModule());
            builder.RegisterModule(new PersistenceModule());

            base.Load(builder);
        }
    }
}
