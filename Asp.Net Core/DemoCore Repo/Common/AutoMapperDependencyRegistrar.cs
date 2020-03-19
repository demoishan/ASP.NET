using Autofac;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;

namespace Common
{
    public class AutoMapperDependencyRegistrar : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.Register(ctx => new MapperConfiguration(cfg => { cfg.AddProfiles(GetType().Assembly); }));
            builder.Register(ctx => ctx.Resolve<MapperConfiguration>().CreateMapper()).As<IMapper>();

            base.Load(builder);
        }

    }
}
