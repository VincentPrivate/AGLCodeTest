using System.Net.Http;

using AGLCodeTest.Functions;
using AGLCodeTest.Services;
using AGLCodeTest.Settings;

using Autofac;

namespace AGLCodeTest.Dependencies
{
    /// <summary>
    /// This represents the module entity for Autofac.
    /// </summary>
    public class AppModule : Module
    {
        /// <summary>
        /// Add registrations to the container builder.
        /// </summary>
        /// <param name="builder">The container builder.</param>
        protected override void Load(ContainerBuilder builder)
        {
            RegisterSettings(builder);
            RegisterServices(builder);
            RegisterFunctions(builder);
        }

        private static void RegisterSettings(ContainerBuilder builder)
        {
            builder.RegisterType<AppSettings>()
                   .As<AppSettings>()
                   .SingleInstance();

            builder.Register(p => new HttpClient())
                   .As<HttpClient>()
                   .SingleInstance();
        }

        private static void RegisterServices(ContainerBuilder builder)
        {
            builder.RegisterAssemblyTypes(typeof(IService).Assembly)
                   .Where(p => p.Name.EndsWith("Service"))
                   .AsImplementedInterfaces()
                   .InstancePerDependency();
        }

        private static void RegisterFunctions(ContainerBuilder builder)
        {
            builder.RegisterAssemblyTypes(typeof(IFunction).Assembly)
                   .Where(t => t.Name.EndsWith("Function"))
                   .AsImplementedInterfaces()
                   .InstancePerDependency();
        }
    }
}