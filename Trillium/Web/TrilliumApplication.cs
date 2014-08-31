namespace Trillium.Web
{
    using System.Reflection;
    using System.Web.Mvc;
    using Autofac;
    using Autofac.Integration.Mvc;
    using Trillium.Models;
    using Trillium.ViewModels;
    using Umbraco.Core;
    using Umbraco.Core.Persistence;

    public class TrilliumApplication : ApplicationEventHandler
    {

        protected override void ApplicationStarted(UmbracoApplicationBase umbracoApplication,
            ApplicationContext applicationContext)
        {
            // Get the Umbraco Database context
            Database db = applicationContext.DatabaseContext.Database;

            //Check if the DB table does NOT exist
            if (!db.TableExist("BlogComments"))
            {
                //Create DB table - and set overwrite to false
                db.CreateTable<BlogComment>(false);
            }

            SetUpDependencyInjection();
        }


        private void SetUpDependencyInjection()
        {
            var builder = new ContainerBuilder();

            builder.RegisterInstance(ApplicationContext.Current).AsSelf();

            // register all controllers found in this assembly
            // builder.RegisterControllers(typeof (TrilliumApplication).Assembly);
            builder.RegisterControllers(Assembly.GetExecutingAssembly());

            // add custom class to the container as Transient instance
            builder.RegisterType<BlogViewModel>();

            // bind abstract IUnitOfWork with specific provider (petapoco, EF, ...)
            // builder.RegisterType<ppUnitOfWOrk>().As<IUnitOfWork>().InstancePerRequest();
            IContainer container = builder.Build();
            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));
        }
    }
}