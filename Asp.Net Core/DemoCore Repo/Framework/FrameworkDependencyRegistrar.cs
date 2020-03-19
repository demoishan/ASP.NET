using Autofac;
using Common.Abstractions;
using Common.Abstractions.Configuration;
using Framework.Builders;
using Framework.Services;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace Framework
{
    public class FrameworkDependencyRegistrar : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<AppSettings>().As<IAppSettings>().SingleInstance();
            builder.Register(c => new HttpClient()).As<HttpClient>();
            builder.RegisterType<HomesUserService>().As<IHomesUserService>();
            base.Load(builder);
        }
    }
}
