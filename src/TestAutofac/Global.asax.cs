using Autofac;
using Autofac.Integration.Mvc;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace TestAutofac
{
    public class MvcApplication : HttpApplication
    {
        internal static IContainer Container;

        protected void Application_Start()
        {
            this.SetDependencyResolver();
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
        }

        public void Application_BeginRequest()
        {
            var service = DependencyResolver.Current.GetService<IBeforeRequestTask>();

            service.Execute();
        }

        public void Application_EndRequest()
        {
            var service = MvcApplication.Container.Resolve<IAfterRequestTask>();
            service.Execute();
        }

        private void SetDependencyResolver()
        {
            var builder = new ContainerBuilder();

            builder.RegisterType<TaskExecutor>()
                .As<IBeforeRequestTask>()
                .As<IAfterRequestTask>()
                .SingleInstance();

            Container = builder.Build();

            // Set Resolver
            DependencyResolver.SetResolver(new AutofacDependencyResolver(Container));
        }
    }

    public interface IBeforeRequestTask
    {
        void Execute();
    }

    public interface IAfterRequestTask
    {
        void Execute();
    }

    public class TaskExecutor : IBeforeRequestTask, IAfterRequestTask
    {
        void IBeforeRequestTask.Execute()
        {
            System.Diagnostics.Debug.Write("Before");
        }

        void IAfterRequestTask.Execute()
        {
            System.Diagnostics.Debug.Write("After");
        }
    }

}
