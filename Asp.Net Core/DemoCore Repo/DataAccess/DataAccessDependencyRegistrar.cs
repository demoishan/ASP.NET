using Autofac;
using Common.Abstractions;
using DataAccess.Repositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataAccess
{
    public class DataAccessDependencyRegistrar : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
           
            builder.RegisterType<ApplicationDefaultsRepository>().As<IApplicationDefaultsRepository>();
            builder.RegisterType<HomesUserRepository>().As<IHomesUserRepository>();
            builder.RegisterType<HomesUserSqlRepository>().As<IHomesUserSqlRepository>();

            base.Load(builder);
        }
    }
}
