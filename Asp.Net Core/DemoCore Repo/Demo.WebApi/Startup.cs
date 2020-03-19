using System;
using System.Collections.Generic;
using System.Linq;
using Authorization;
using Autofac;
using Autofac.Configuration;
using Autofac.Extensions.DependencyInjection;
using Common;
using DataAccess;
using DataAccess.Repositories;
using Framework;
using Framework.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using System.Text;


namespace Demo.WebApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.AddLogging();
            services.AddMemoryCache();
            services.AddMvc();

            ConfigurePolicies(services);

            var configBuilder = new ConfigurationBuilder()
             .AddJsonFile("appsettings.json")
             .Build();

            var builder = new ContainerBuilder();

            builder.RegisterModule(new ConfigurationModule(configBuilder));
            builder.RegisterModule(new CommonDependencyRegistrar());
            builder.RegisterModule(new FrameworkDependencyRegistrar());
            builder.RegisterModule(new DataAccessDependencyRegistrar());
            builder.RegisterModule(new WebApiDependencyRegistrar());
            builder.RegisterModule(new AutoMapperDependencyRegistrar());

            builder.Populate(services);

            var container = builder.Build();
            return container.Resolve<IServiceProvider>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        //public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        //{
        //    if (env.IsDevelopment())
        //    {
        //        app.UseDeveloperExceptionPage();
        //    }

        //    app.UseMvc();
        //}
        private void ConfigurePolicies(IServiceCollection services)
        {
            var appSettings = new Framework.Builders.AppSettings();
            var repository = new AccessControlRepository(appSettings, new Common.Factories.AshleyHsDbConnectionFactory(appSettings));
            AccessControlService accessControlService = new AccessControlService(repository);
            List<Common.Models.SecureResource> resources = accessControlService.GetResourceListAsync().Result as List<Common.Models.SecureResource>;

            foreach (var resource in resources)
            {
                foreach (var accessControl in resource.AccessControl) //not efficient but the numbers should be small
                {
                    var roles = accessControl.Value.Select(role => role.Name).ToList();

                    services.AddAuthorization(options =>
                        options.AddPolicy($"{resource.Name.ToLower()}_{accessControl.Key.ToLower()}_roles",
                        policy =>
                        {
                            policy.AuthenticationSchemes.Add(JwtBearerDefaults.AuthenticationScheme);
                            policy.Requirements.Add(new RoleListRequirement(roles));
                        }
                    ));
                }
            }
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            app.UseCors(
                options => options
                    .AllowAnyOrigin()
                    .AllowAnyHeader()
                    .AllowAnyMethod()
            );

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            var logger = new LoggerConfiguration()
                            .ReadFrom.Configuration(Configuration)
                            .CreateLogger();
            loggerFactory.AddSerilog(logger);

            app.UseAuthentication();

            app.UseMvc();
        }
    }
}
