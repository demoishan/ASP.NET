using Autofac;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Demo.WebApi
{
    public class WebApiDependencyRegistrar : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            //builder.RegisterType<PurchaseOrderValidator>().As<IValidator<PurchaseOrder>>();
            //builder.RegisterType<PurchaseOrderLineValidator>().As<IValidator<PurchaseOrderLine>>();
            //builder.RegisterType<VendorValidator>().As<IValidator<Vendor>>();
            base.Load(builder);
        }
    }
}
