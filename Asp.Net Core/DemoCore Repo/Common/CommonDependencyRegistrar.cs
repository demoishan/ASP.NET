using Autofac;
using Common.Models;
using Common.Abstractions.Data;
using Common.Factories;
namespace Common
{
    public class CommonDependencyRegistrar : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<AshleyDbConnectionFactory>().As<IAshleyDbConnectionFactory>();
            builder.RegisterType<AshleyHsDbConnectionFactory>().As<IAshleyHsDbConnectionFactory>();
            builder.RegisterType<HomesUser>();

            //Register types here
            base.Load(builder);
        }
    }
}
