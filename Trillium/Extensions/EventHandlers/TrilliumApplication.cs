namespace Trillium.Extensions.EventHandlers
{
    using System.Web.Mvc;
    using Autofac;
    using Autofac.Integration.Mvc;
    using Trillium.ViewModels;
    using Umbraco.Core;

    public class TrilliumApplication : ApplicationEventHandler
    {
        protected override void ApplicationStarted(UmbracoApplicationBase umbracoApplication,
            ApplicationContext applicationContext)
        {
            SetupDependencyInjection();
        }

        private void SetupDependencyInjection()
        {
            var builder = new ContainerBuilder();

            // register all controllers found in this assembly
            builder.RegisterControllers(typeof (TrilliumApplication).Assembly);

            // add custom class to the container as Transient instance
            builder.RegisterType<BlogViewModel>();

            IContainer container = builder.Build();

            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));
        }
    }
}